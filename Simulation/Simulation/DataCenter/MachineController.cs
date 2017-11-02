using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.LocationStrategies;
using Simulation.SimulationController;

namespace Simulation.DataCenter
{
    public class MachineController : IMachinePowerController,IStart
    {
        public bool Started { get; set; } = true;

        public MachineTable MachineTable { get; set; }
        private NetworkSwitch Network { get; set; }
        private MachineTable ReadyMachineTable { get; set; }
        public Strategies Strategy { get; set; }
        private UtilizationTable DataHolder { get; set; }
        public bool StartingMachine { get; set; } = false;


        private object _lock = new object();

        public MachineController(UtilizationTable holder, MachineTable machineTable,NetworkSwitch network,Strategies strategy)
        {
            MachineTable = machineTable;
            Network = network;
            ReadyMachineTable = new MachineTable();
            Strategy = strategy;
            DataHolder = holder;
            Task t = new Task(async () =>
            {
                while (Started)
                {
                    var currentTotal = MachineTable.GetMachinesCount()-1;
                    var mcount = Convert.ToInt32(currentTotal * 0.05);
                    var rcount = ReadyMachineTable.GetMachinesCount();
                    if (currentTotal+rcount<=((int)Global.SimulationSize-1))
                    {
                        if (mcount > rcount)
                        {
                            CreateANewMachine();
                        }
                        else if (mcount < rcount)
                        {
                            KillOldMachine();
                        }
                    }
                    else
                    {
                        
                    }
                    await Task.Delay(Global.Second);
                }
            });
            t.Start();
        }


        private void CreateANewMachine()
        {
            if (!StartingMachine)
            {
                Task t = new Task(async() =>
                {
                    var l = Global.DataCenterHostConfiguration;
                    var machine = new HostMachine(RandomNumberGenerator.GetHostRandomNumber(), l, Network,
                        Global.LoadPrediction, Strategy);
                    ReadyMachineTable.AddMachine(machine.MachineId, machine);
                    StartingMachine = true;
                    await Task.Delay(Global.Second * 60);
                    StartingMachine = false;
                });
                t.Start();
            }

        }
        private void KillOldMachine()
        {
            if (!StartingMachine)
            {
                Task t = new Task(async () =>
                {
                    var l = Global.DataCenterHostConfiguration;
                    var machine = ReadyMachineTable.GetAllMachines().Last();
                    ReadyMachineTable.RemoveMachine(machine.MachineId);
                    StartingMachine = true;
                    await Task.Delay(Global.Second * 60);
                    StartingMachine = false;
                });
                t.Start();
            }
        }

        public void StartSimulation()
        {
            lock (_lock)
            {
                //NetworkSwitch.Started = true;
                foreach (var machine in MachineTable.GetAllMachines().Skip(1))
                {
                    DataHolder.SetUtilization(machine.MachineId, UtilizationStates.Normal);

                    machine.StartMachine();
                }
                MachineTable.GetMachineById(0).StartMachine();
            }
        }

        public void EndSimulation()
        {
            lock (_lock)
            {
                foreach (var machine in MachineTable.GetAllMachines())
                {
                    machine.StopMachine();
                }
                //MachineTable.GetMachineById(0).StopMachine();
                Started = false;
            }
        }

        public void AddHost(Machine machine)
        {
            lock (_lock)
            {
                MachineTable.AddMachine(machine.MachineId, machine);
            }
        }

        public void PowerOnHost()
        {
            lock (_lock)
            {
                if (ReadyMachineTable.GetMachinesCount() != 0)
                {
                    Machine machine = ReadyMachineTable.GetAllMachines().First();
                    ReadyMachineTable.RemoveMachine(machine.MachineId);
                    MachineTable.AddMachine(machine.MachineId, machine);
                    DataHolder.SetUtilization(machine.MachineId, UtilizationStates.UnderUtilization);
                    machine.StartMachine();
                }
            }
        }

        

        public int GetMachinesCount()
        {
            lock (_lock)
            {
                return MachineTable.GetMachinesCount();
            }
        }

        public void PowerOffHost(int machineId)
        {
            lock (_lock)
            {
                var machine = MachineTable.GetMachineById(machineId);
                if ((machine as HostMachine).GetContainersCount() != 0)
                {
                    //return false;
                    throw new NotImplementedException();
                }
                else
                {
                    machine.StopMachine();
                    MachineTable.RemoveMachine(machineId);
                    DataHolder.RemoveUtilization(machineId);
                    ReadyMachineTable.AddMachine(machineId,machine);
                    Console.WriteLine($"( ) Power off Host #{machineId}");

                    //return true;
                }
            }
        }

    }

    public interface IMachinePowerController
    {
        void PowerOnHost();
        void PowerOffHost(int senderId);
    }
}
