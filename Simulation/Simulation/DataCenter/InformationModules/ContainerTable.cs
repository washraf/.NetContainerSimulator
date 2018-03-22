using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;
using Simulation.DataCenter.Containers;
using Simulation.Configuration;

namespace Simulation.DataCenter.InformationModules
{
    public class ContainerTable
    {
        public int MachineId { get; set; }

        public ContainerTable(int machineId)
        {
            MachineId = machineId;
            ContainerType = ContainersType.N;
        }

        protected object _lock = new object();
        private Container MigratedContainer { get; set; }
        protected Dictionary<int, Container> ContainersTable { get; set; } = new Dictionary<int, Container>();
        public ContainersType ContainerType { get; protected set; }

        public List<Container> GetAllContainers()
        {
            lock (_lock)
            {
                return ContainersTable.Select(x => x.Value).ToList();
            }
        }
        public int GetContainersCount()
        {
            lock (_lock)
            {
                return ContainersTable.Count;
            }
        }
        public Container GetContainerById(int conId)
        {
            lock (_lock)
            {
                return ContainersTable[conId];
            }
        }
        public virtual void AddContainer(int containerId, Container container)
        {
            lock (_lock)
            {
                ContainersTable.Add(containerId, container);
            }
        }
        public void LockContainer(int conId)
        {
            lock (_lock)
            {
                var container = ContainersTable[conId];
                ContainersTable.Remove(conId);
                if(container == null)
                    throw new ArgumentException("No SUch con");
                MigratedContainer = container;
            }
        }
        public void FreeLockedContainer()
        {
            lock (_lock)
            {
                if (MigratedContainer == null) 
                    throw new NotImplementedException("How come");
                else
                {
                    MigratedContainer = null;
                    _hashSet.Clear();
                }
            }
        }
        public void UnLockContainer()
        {
            lock (_lock)
            {
                ContainersTable.Add(MigratedContainer.ContainerId, MigratedContainer);
                MigratedContainer = null;
                throw new NotImplementedException("Should Never be called");
            }
        }
        private HashSet<int> _hashSet = new HashSet<int>();  

        /// <summary>
        /// Update to different conditions
        /// </summary>
        /// <returns></returns>
        public Container SelectContainerByCondition()
        {
            lock (_lock)
            {
                if (!ContainersTable.Any())
                {
                    return null;
                }
                if (ContainersTable.Count == _hashSet.Count)
                {
                    _hashSet.Clear();
                }
                Container container = ContainersTable.Values.OrderByDescending(
                        x => x.GetContainerPredictedLoadInfo().VolumeToSizeRatioToMigrationsCount)
                        .SkipWhile(x=>_hashSet.Contains(x.ContainerId))
                        .First();
                _hashSet.Add(container.ContainerId);
                return container;
            }
        }

        public Load GetContainersTotalNeededLoad()
        {
            lock (_lock)
            {
                var totalLoad = new Load(0, 0, 0);
                int cccc = ContainersTable.Count;
                foreach (var container in ContainersTable.Values)
                {
                    var containerRealLoad = container.GetContainerNeededLoadInfo().CurrentLoad;
                    totalLoad += containerRealLoad;
                }
                if (MigratedContainer != null)
                {
                    totalLoad += MigratedContainer.GetContainerNeededLoadInfo().CurrentLoad;
                }
                return totalLoad;
            }
        }

        public Load GetContainersTotalPredictedLoad()
        {
            lock (_lock)
            {
                var totalLoad = new Load(0, 0, 0);

                foreach (var container in ContainersTable.Values)
                {
                    var containerPredictedLoad = container.GetContainerPredictedLoadInfo().CurrentLoad;
                    totalLoad += containerPredictedLoad;
                }
                if (MigratedContainer != null)
                {
                    totalLoad += MigratedContainer.GetContainerPredictedLoadInfo().CurrentLoad;
                }
                return totalLoad;
            }
        }

        public List<int> CalculateSlaViolations(Load loadDifference)
        {
            lock (_lock)
            {
                var containersLoad = GetContainersTotalNeededLoad();
                List<int> slaViolations = new List<int>();
                foreach (var container in ContainersTable.Values)
                {
                    if (container.IsViolated(loadDifference, containersLoad))
                    {
                        slaViolations.Add(container.ContainerId);
                    }
                }
                return slaViolations;
            }
        }

        public List<ContainerLoadInfo> GetAllContainersLoadInfo()
        {
            lock (_lock)
            {
                return ContainersTable.Values.Select(x => x.GetContainerPredictedLoadInfo()).ToList();
            }
        }
    }
}
