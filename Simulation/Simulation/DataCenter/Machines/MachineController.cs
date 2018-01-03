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
using Simulation.DataCenter.Machines;
using Simulation.DataCenter.Network;
using Simulation.DataCenter.Core;

namespace Simulation.DataCenter.Machines
{
    public class MachineController : IMachinePowerController, IStart
    {
        public bool Started { get; set; } = true;
        public MachineTable MachineTable { get; set; }
        private NetworkSwitch Network { get; set; }
        private MachineTable ReadyMachineTable { get; set; }
        private MachineTable PoweredOffMachinesTable { get; set; }
        public Strategies Strategy { get; set; }
        public ContainersType ContainerTypes { get; }
        private UtilizationTable DataHolder { get; set; }
        public bool StartingMachine { get; set; } = false;
        private object _lock = new object();

        public MachineController(UtilizationTable holder, MachineTable machineTable, NetworkSwitch network, Strategies strategy, ContainersType containerTypes)
        {
            MachineTable = machineTable;
            Network = network;
            ReadyMachineTable = new MachineTable();
            PoweredOffMachinesTable = new MachineTable();

            Strategy = strategy;
            ContainerTypes = containerTypes;
            DataHolder = holder;
            Task t = new Task(async () =>
            {
                while (Started)
                {
                    var currentTotal = MachineTable.GetHostsCount();
                    var mcount = Convert.ToInt32(currentTotal * 0.05);
                    var rcount = ReadyMachineTable.GetHostsCount();
                    if (!StartingMachine)
                    {
                        if (mcount > rcount && PoweredOffMachinesTable.GetHostsCount() > 0)
                        {
                            await WakeIdleHost();
                        }
                        else if (mcount < rcount)
                        {
                            await PowerOffExtraMachines();
                        }
                    }
                    //else
                    //{

                    //}
                    await Task.Delay(Global.Second);
                }
            });
            t.Start();
        }


        private async Task WakeIdleHost()
        {
            if (!StartingMachine)
            {
                //Task t = new Task(async () =>
                //{
                //var l = Global.DataCenterHostConfiguration;
                StartingMachine = true;
                var machine = PoweredOffMachinesTable.GetAllMachines().Last();
                await Task.Delay(Global.Second * 60);
                ReadyMachineTable.AddMachine(machine.MachineId, machine);
                PoweredOffMachinesTable.RemoveMachine(machine.MachineId);
                StartingMachine = false;
                //});
                //t.Start();
            }

        }
        private async Task PowerOffExtraMachines()
        {
            if (!StartingMachine)
            {
                //Task t = new Task(async () =>
                //{
                //var l = Global.DataCenterHostConfiguration;
                StartingMachine = true;
                var machine = ReadyMachineTable.GetAllMachines().Last();
                ReadyMachineTable.RemoveMachine(machine.MachineId);
                PoweredOffMachinesTable.AddMachine(machine.MachineId, machine);
                await Task.Delay(Global.Second * 60);
                StartingMachine = false;
                //});
                //t.Start();
            }
        }

        public void StartSimulation()
        {
            lock (_lock)
            {
                //NetworkSwitch.Started = true;
                foreach (var machine in MachineTable.GetAllMachines().Skip(1).Where(x => x.MachineId < int.MaxValue))
                {
                    DataHolder.SetUtilization(machine.MachineId, UtilizationStates.Normal);

                    machine.StartMachine();
                }
                MachineTable.GetMachineById(0).StartMachine();
                if (ContainerTypes == ContainersType.D)
                {
                    MachineTable.GetMachineById(int.MaxValue).StartMachine();
                }
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

        public void AddMachine(Machine machine)
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
                if (ReadyMachineTable.GetHostsCount() != 0)
                {
                    Machine machine = ReadyMachineTable.GetAllMachines().First();
                    ReadyMachineTable.RemoveMachine(machine.MachineId);
                    MachineTable.AddMachine(machine.MachineId, machine);
                    DataHolder.SetUtilization(machine.MachineId, UtilizationStates.UnderUtilization);
                    machine.StartMachine();
                }
            }

        }



        //public int GetMachinesCount()
        //{
        //    lock (_lock)
        //    {
        //        return MachineTable.GetMachinesCount();
        //    }
        //}

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
                    //PoweredOffMachinesTable.AddMachine(machine.MachineId, machine);
                    MachineTable.RemoveMachine(machineId);
                    DataHolder.RemoveUtilization(machineId);
                    ReadyMachineTable.AddMachine(machineId, machine);
                    //Console.WriteLine($"( ) Power off Host #{machineId}");

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
