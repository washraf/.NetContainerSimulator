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

namespace Simulation.SimulationController
{
    public class SimulationController : ISimulationController
    {
        public Strategies CurrentStrategy { get; set; }
        public SimulationSize CurrentSimulationSize { get; private set; }
        public StartUtilizationPercent UtilizationPercent { get; set; }
        public LoadPrediction CurrentPrediction { set; get; }
        public LoadChangeAction ChangeAction { get; set; }

        public MachineTable MachineTableObject { get; private set; }
        public MachineController MachineControllerObject { get; private set; }
        public NetworkSwitch NetworkSwitchObject { get; private set; }
        public IAccountingModule AccountingModuleObject { get; private set; }

        public UtilizationTable UtilizationTable { get; private set; }

        public SimulationController(Strategies strategies, SimulationSize size, StartUtilizationPercent utilizationPercent, LoadPrediction currentPrediction, LoadChangeAction changeAction)
        {
            CurrentStrategy = strategies;
            CurrentSimulationSize = size;
            UtilizationPercent = utilizationPercent;
            ChangeAction = changeAction;
            CurrentPrediction = currentPrediction;
            MachineTableObject = new MachineTable();
            UtilizationTable = new UtilizationTable();
            AccountingModuleObject = new AccountingModule(MachineTableObject);
            NetworkSwitchObject = new NetworkSwitch(MachineTableObject, AccountingModuleObject);
            MachineControllerObject = new MachineController(UtilizationTable, MachineTableObject,NetworkSwitchObject, Global.CurrentStrategy);
        }

        public void StartSimulation()
        {
            //Global.CommonLoadManager.Clear();
            Global.CommonLoadManager = new CommonLoadManager(AccountingModuleObject);
            Machine master = new MasterMachine(NetworkSwitchObject, MachineControllerObject, UtilizationTable,
                Global.CurrentStrategy);
            MachineControllerObject.AddHost(master);
            var h = new Load(Global.DataCenterHostConfiguration);

            for (int i = 1; i <= (int) CurrentSimulationSize; i++)
            {
                MachineControllerObject.AddHost(new HostMachine(RandomNumberGenerator.GetHostRandomNumber(), h,
                    NetworkSwitchObject, Global.LoadPrediction, Global.CurrentStrategy));
            }

            //Should Be replaceي with Load Controller
            //hostController.RemoveHost(2);
            var hosts = MachineTableObject.GetAllMachines();
            //Assign Load
            //var startLoad = Global.ContainerLoadNormal;
            //var endLoad = Global.ContainerLoadPostNormal;

            for (int i = 1; i <= (int) CurrentSimulationSize; i++)
            {
                //int conId = RandomNumberGenerator.GetContainerRandomNumber();
                //Random r = new Random();
                List<Load> loads = LoadGenerator.GenerateLoadList(UtilizationPercent, CurrentSimulationSize);
                foreach (var load in loads)
                {
                    var conId = RandomNumberGenerator.GetContainerRandomNumber();
                    //var i = r.Next(1, (int)CurrentSimulationSize + 1);
                    (hosts[i] as HostMachine).AddContainer(new Container(conId, load, CurrentPrediction));
                }
            }

            MachineControllerObject.StartSimulation();
            //StartWaveSimmulationAction((int)(Global.GetSimulationTime *0.6),Global.SecondWaveChange);
            //AccountingModuleObject.StartCounting();
            var done = false;
            for (int x = 0; x <= Global.GetSimulationTime; x += Global.AccountTime)
            {
                AccountingModuleObject.ReadCurrentState();
                Thread.Sleep(Global.AccountTime);
                if (x >= Global.GetSimulationTime/2 && !done)
                {
                    if (ChangeAction == LoadChangeAction.VreyHighOpposite)
                    {
                        StartWaveSimmulationAction(LoadChangeAction.VeryHightBurst, m => true, m => m.ContainerId % 2 == 0);
                        StartWaveSimmulationAction(LoadChangeAction.VeryHightDrain, m => true, m => m.ContainerId % 2 == 1);
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
            var machines = MachineTableObject.GetAllMachines().Skip(1).Where(hostsFunc).ToList();

            for (int i = 0; i < machines.Count; i++)
            {
                var host = machines[i] as HostMachine;
                host.ChangeAllContainersLoad(changeMethod, contsFunc);
            }
        }

        private void EndSimulation()
        {
            //AccountingModuleObject.
            
            AccountingModuleObject.StopCounting();
            MachineControllerObject.EndSimulation();
            RandomNumberGenerator.ClearRandomNumber();
        }

        
    }
}
