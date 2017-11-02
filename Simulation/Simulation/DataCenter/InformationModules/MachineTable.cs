using System.Collections.Generic;
using System.Linq;

namespace Simulation.DataCenter.InformationModules
{
    public class MachineTable : ISwitchTable
    {
        private readonly Dictionary<int, Machine> _machineTable = new Dictionary<int, Machine>();

        private object _lock = new object();

        public void AddMachine(int id, Machine machine)
        {
            lock (_lock)
            {
                _machineTable.Add(id, machine);
            }
        }

        public void RemoveMachine(int id)
        {
            lock (_lock)
            {
                _machineTable.Remove(id);
            }
        }

        public bool ValidateMachineId(int id)
        {

            lock (_lock)
            {
                return (_machineTable.ContainsKey(id) || id == -1);
            }

        }

        public Machine GetMachineById(int id)
        {
            lock (_lock)
            {
                if (_machineTable.ContainsKey(id))
                    return _machineTable[id];
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
        }

        public List<int> GetAllMachineIds(int skip)
        {
            lock (_lock)
            {
                return _machineTable.Keys.Where(x => x!=0).Where(x=>x != skip).ToList();
            }
        }

        public List<Machine> GetAllMachines()
        {
            lock (_lock)
            {
                return _machineTable.Values.ToList();
            }
        }

        public int GetMachinesCount()
        {
            lock (_lock)
            {
                return _machineTable.Count;
            }
        }

    }
}