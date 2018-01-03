using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.Modules.LoadManagement;
using Simulation.DataCenter.Containers;
using Simulation.DataCenter.Machines;
using Simulation.DataCenter.Network;

namespace Simulation.SimulationController
{
    public class SimulationController : ISimulationController
    {
        private readonly Strategies _currentStrategy;
        public SimulationSize CurrentSimulationSize { get; private set; }
        public StartUtilizationPercent UtilizationPercent { get; set; }
        private LoadPrediction _currentLoadPrediction;
        private readonly ContainersType containerType;

        public LoadChangeAction ChangeAction { get; set; }
        public TestedHosts TestedHostsCount { get; }
        public MachineTable MachineTableObject { get; private set; }
        public MachineController MachineControllerObject { get; private set; }

        private NetworkSwitch _networkSwitchObject;

        public IAccountingModule AccountingModuleObject { get; private set; }

        public UtilizationTable UtilizationTable { get; private set; }

        private readonly MachineFactory _masterFactory;
        private readonly MachineFactory _hostFactory;
        private readonly MachineFactory _registryFactory;
        private readonly ContainerFactory _containerFactory;

        public SimulationController(Strategies strategy, SimulationSize size,
            StartUtilizationPercent utilizationPercent,
            LoadPrediction currentPrediction, LoadChangeAction changeAction,
            TestedHosts testedHosts, ContainersType containerType)
        {
            _currentStrategy = strategy;
            CurrentSimulationSize = size;
            UtilizationPercent = utilizationPercent;
            ChangeAction = changeAction;
            TestedHostsCount = testedHosts;
            this.containerType = containerType;
            _currentLoadPrediction = currentPrediction;
            MachineTableObject = new MachineTable();
            UtilizationTable = new UtilizationTable();
            AccountingModuleObject = new AccountingModule(MachineTableObject, UtilizationTable,_currentStrategy,CurrentSimulationSize, UtilizationPercent,ChangeAction,currentPrediction,testedHosts,containerType);
            _networkSwitchObject = new NetworkSwitch(MachineTableObject, AccountingModuleObject);

            MachineControllerObject = new MachineController(UtilizationTable, MachineTableObject,_networkSwitchObject, _currentStrategy,containerType);
            _masterFactory = new MasterFactory(_networkSwitchObject, MachineControllerObject, UtilizationTable,
                _currentStrategy, TestedHostsCount);
            var h = new Load(Global.DataCenterHostConfiguration);
            _hostFactory = new HostFactory(h,
                    _networkSwitchObject, _currentLoadPrediction, _currentStrategy,containerType);
            _registryFactory = new RegistryFactory(_networkSwitchObject, CurrentSimulationSize);
            _containerFactory = new ContainerFactory(containerType,CurrentSimulationSize,currentPrediction);
        }

        public void StartSimulation()
        {
            //Common Load Manager for Zhao (refactor to be a network resource)
            Global.CommonLoadManager = new CommonLoadManager(AccountingModuleObject);

            //Master Machine
            Machine master = _masterFactory.GetMachine();
            MachineControllerObject.AddMachine(master);

            //Host Machines
            for (int i = 1; i <= (int)CurrentSimulationSize; i++)
            {
                MachineControllerObject.AddMachine(_hostFactory.GetMachine());
            }
            //Add registry when needed
            if (containerType == ContainersType.D)
            {
                Machine registry = _registryFactory.GetMachine();
                MachineControllerObject.AddMachine(registry);
            }

            var hosts = MachineTableObject.GetAllMachines();

            for (int i = 1; i <= (int) CurrentSimulationSize; i++)
            {
                List<Load> loads = LoadGenerator.GenerateLoadList(UtilizationPercent, CurrentSimulationSize);
                foreach (var load in loads)
                {
                    
                    (hosts[i] as HostMachine).AddContainer( _containerFactory.GetContainer(load));
                }
            }

            MachineControllerObject.StartSimulation();

            var done = false;
            for (int x = 0; x <= Global.GetSimulationTime; x += Global.AccountTime)
            {
                AccountingModuleObject.ReadCurrentState();
                Thread.Sleep(Global.AccountTime);
                if (x >= Global.GetSimulationTime/2 && !done)
                {
                    if (ChangeAction == LoadChangeAction.Opposite)
                    {
                        StartWaveSimmulationAction(LoadChangeAction.Burst, m => true, m => m.ContainerId % 2 == 0);
                        StartWaveSimmulationAction(LoadChangeAction.Drain, m => true, m => m.ContainerId % 2 == 1);
                    }
                    else
                    {
                        StartWaveSimmulationAction(ChangeAction, m =>true, m => m.ContainerId%2==0);
                    }
                    done = true;
                }
            }
            EndSimulation();
        }

        private void StartWaveSimmulationAction(LoadChangeAction changeMethod, Func<Machine, bool> hostsFunc,
            Func<Container, bool> contsFunc)
        {
            var machines = MachineTableObject.GetAllMachines().Skip(1).Where(x=>x.MachineId<int.MaxValue).Where(hostsFunc).ToList();

            for (int i = 0; i < machines.Count; i++)
            {
                var host = machines[i] as HostMachine;
                host.ChangeAllContainersLoad(changeMethod, contsFunc);
            }
        }

        private void EndSimulation()
        {
            AccountingModuleObject.StopCounting();
            MachineControllerObject.EndSimulation();
            RandomNumberGenerator.ClearRandomNumber();
        }

        
    }
}
