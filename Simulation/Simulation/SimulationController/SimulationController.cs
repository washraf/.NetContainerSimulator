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
using Simulation.Factories;
using System.Diagnostics;

namespace Simulation.SimulationController
{
    public class SimulationController : ISimulationController
    {
        private RunConfiguration CurrentConfiguration;

        
        public MachineTable MachineTableObject { get; private set; }
        public MachineController MachineControllerObject { get; private set; }

        private NetworkSwitch _networkSwitchObject;

        public IAccountingModule AccountingModuleObject { get; private set; }

        public UtilizationTable UtilizationTable { get; private set; }

        private readonly MachineFactory _masterFactory;
        private readonly MachineFactory _hostFactory;
        private readonly MachineFactory _registryFactory;
        private readonly ContainerFactory _containerFactory;

        public SimulationController( RunConfiguration configuration)
        {
            CurrentConfiguration = configuration;
            MachineTableObject = new MachineTable();
            UtilizationTable = new UtilizationTable();
            AccountingModuleObject = new AccountingModule(MachineTableObject, UtilizationTable,
                CurrentConfiguration);
            _networkSwitchObject = new NetworkSwitch(MachineTableObject, AccountingModuleObject);

            MachineControllerObject 
                = new MachineController(UtilizationTable, MachineTableObject, CurrentConfiguration.ContainersType);
            _masterFactory 
                = new MasterFactory(_networkSwitchObject, MachineControllerObject, UtilizationTable,
                CurrentConfiguration.Strategy,CurrentConfiguration.PushAuctionType,CurrentConfiguration.PullAuctionType, CurrentConfiguration.SchedulingAlgorithm, CurrentConfiguration.TestedHosts);
            var h = new Load(Global.DataCenterHostConfiguration);
            _hostFactory = new HostFactory(h,
                    _networkSwitchObject, CurrentConfiguration.LoadPrediction, CurrentConfiguration.Strategy, CurrentConfiguration.ContainersType, configuration.SimulationSize);
            _registryFactory = new RegistryFactory(_networkSwitchObject, configuration.SimulationSize);
            _containerFactory = new ContainerFactory(CurrentConfiguration.ContainersType,configuration.SimulationSize,configuration.LoadPrediction);
        }

        public void StartSimulation()
        {
            //Common Load Manager for Zhao (refactor to be a network resource)
            Global.CommonLoadManager = new CommonLoadManager(AccountingModuleObject);

            //Master Machine
            MasterMachine masterMachine = (MasterMachine) _masterFactory.GetMachine();
            MachineControllerObject.AddMachine(masterMachine);

            //Host Machines
            for (int i = 1; i <= (int)CurrentConfiguration.SimulationSize; i++)
            {
                MachineControllerObject.AddMachine(_hostFactory.GetMachine());
            }
            //Add registry when needed
            if (CurrentConfiguration.ContainersType == ContainersType.D)
            {
                Machine registry = _registryFactory.GetMachine();
                MachineControllerObject.AddMachine(registry);
            }

            var hosts = MachineTableObject.GetAllMachines();

            for (int i = 1; i <= (int)CurrentConfiguration.SimulationSize; i++)
            {
                List<Load> loads = LoadGenerator.GenerateLoadList(CurrentConfiguration.StartPercent, CurrentConfiguration.SimulationSize);
                foreach (var load in loads)
                {
                    
                    (hosts[i] as HostMachine).AddContainer( _containerFactory.GetContainer(load));
                }
            }

            MachineControllerObject.StartSimulation();

            var done = false;
            int c = 0;
            for (int x = 0; x <= Global.GetSimulationTime; x += Global.AccountTime,c++)
            {
                Debug.WriteLine($"Time = {x}/{Global.GetSimulationTime}");
                AccountingModuleObject.ReadCurrentState();
                Thread.Sleep(Global.AccountTime);
                if (x >= Global.GetSimulationTime/2 && !done)
                {
                    if (CurrentConfiguration.ChangeAction == LoadChangeAction.Opposite)
                    {
                        StartWaveSimmulationAction(LoadChangeAction.Burst, m => true, m => m.ContainerId % 2 == 0);
                        StartWaveSimmulationAction(LoadChangeAction.Drain, m => true, m => m.ContainerId % 2 == 1);
                    }
                    else if(CurrentConfiguration.ChangeAction == LoadChangeAction.None)
                    {

                    }
                    else
                    {
                        StartWaveSimmulationAction(CurrentConfiguration.ChangeAction, m =>true, m => m.ContainerId%2==0);
                    }
                    done = true;
                }
                //Add container to queue
                if(CurrentConfiguration.ChangeAction == LoadChangeAction.None)
                {
                    int s = 0;
                    Predicate<int> anotherTest = new Predicate<int>(n=>true);
                    switch (CurrentConfiguration.SimulationSize)
                    {
                        case SimulationSize.Twenty:
                            s = 3;
                            anotherTest = n => n % 4 == 0;
                            break;
                        case SimulationSize.Fifty:
                            s = 2;
                            break;
                        case SimulationSize.Hundred:
                            s = 7;
                            anotherTest = n => n % 2 == 0;
                            break;
                        case SimulationSize.TwoHundred:
                            s = 7;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    for (int i = 0; i < s & anotherTest(c); i++)
                    {
                        masterMachine.AddContainer(_containerFactory.GetContainer(LoadGenerator.GetRandomLoad()));

                    }
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
