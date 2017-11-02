using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.DataCenter.InformationModules
{
    /// <summary>
    /// Utilization table is a key value store for all the Hosts of the systems 
    /// Utilization states <see cref="UtilizationStates"/>
    /// </summary>
    public class UtilizationTable
    {
        private readonly Dictionary<int, UtilizationStates> _utilization = new Dictionary<int, UtilizationStates>();
        private object _lock = new object();
        public void SetUtilization(int id, UtilizationStates state)
        {
            lock (_lock)
            {
                _utilization[id] = state;

            }
        }

        public UtilizationStates GetUtilization(int id)
        {
            lock (_lock)
            {
                return _utilization[id];
            }
        }
        public void RemoveUtilization(int id)
        {
            lock (_lock)
            {
                _utilization.Remove(id);
            }
        }

        /// <summary>
        /// Implement Ordering and In Order Proping Limit
        /// SelfNote: Cannot remember this function
        /// </summary>
        /// <param name="state"></param>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public List<int> GetCandidateHosts(UtilizationStates state, int senderId)
        {
            lock (_lock)
            {
                var initial = _utilization
                    .Where(x => x.Value == state && x.Key != senderId)
                    .Select(x => x.Key)
                    .ToList();
                var result = new Dictionary<int, int>();
                Random rand = new Random();

                for (int i = 0; i < initial.Count; i++)
                {
                    int r = rand.Next(0, 100000);
                    if (result.ContainsKey(r))
                    {
                        i--;
                    }
                    else
                    {
                        result.Add(r, initial[i]);
                    }
                }
                return result.OrderBy(x => x.Key).Select(x => x.Value).ToList();
                //return initial;
            }
        }
    }
}
