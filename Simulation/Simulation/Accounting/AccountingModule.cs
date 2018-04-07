using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.Messages;
using Simulation.DataCenter.Machines;
using Simulation.Measure;
using Simulation.SimulationController;
using System.Diagnostics;

namespace Simulation.Accounting
{
    public class AccountingModule:IAccountingModule
    {
        public AccountingModule(MachineTable machineTable, UtilizationTable utilizationTable,
            RunConfiguration configuration)
        {
            ClearInformation();
            _machineTable = machineTable;
            this._utilizationTable = utilizationTable;
            MeasureHolder = new MeasureValueHolder(configuration);
        }
        private object _lock = new object();

        Dictionary<MessageTypes, int> _currentRequests = new Dictionary<MessageTypes, int>();
        private double communicatedSize = 0;

        public MeasureValueHolder MeasureHolder { get;} 
       
        private readonly MachineTable _machineTable;
        private readonly UtilizationTable _utilizationTable;

        public void StopCounting()
        {
            CollectMigrationCounts();
            CollectImagePull();
        }

        public void ReadCurrentState()
        {
            lock (_lock)
            {
                List<HostLoadInfo> loads = GetLoadList(false);
                List<HostLoadInfo> predictedLoads = GetLoadList(true);

                double neededtAvg = CalculateVolumeAverage(loads);
                double min = CalculateNeededVolumeMin(loads);
                double max = CalculateNeededVolumeMax(loads);

                double predictedAvg = CalculateVolumeAverage(predictedLoads);

                CalculateOutOfBoundHosts(out int underUtilized, out int overUtilizedHosts,out int normal,out int evacuating);

                int slaViolations = CalculateSlaViolations();

                double imagePulls = _currentRequests[MessageTypes.ImagePullRequest];
                var mvalues = new MeasuresValues(
                    _currentRequests[MessageTypes.PushRequest],
                    _currentRequests[MessageTypes.PullRequest],
                    CalculateIdealHostsCount(loads),
                    loads.Count,
                    _currentRequests[MessageTypes.MigrateContainerRequest],
                    _currentRequests.Values.Sum(),
                    AccountingHelpers.CalculateEntropy(loads),
                    AccountingHelpers.CalculateEntropy(predictedLoads),
                    _currentRequests[MessageTypes.PushLoadAvailabilityRequest],
                    _currentRequests[MessageTypes.PullLoadAvailabilityRequest], neededtAvg, predictedAvg,
                    min,
                    max,
                    underUtilized,
                    overUtilizedHosts,
                    normal,
                    evacuating,
                    slaViolations,
                    loads.PowerConsumption(),
                    loads.StandardDeviation(),
                    imagePulls,
                    communicatedSize
                    );
                
                MeasureHolder.HostMeasureValuesList.Add(new HostMeasureValues(loads));
                MeasureHolder.MeasuredValuesList.Add(mvalues);
                //Debug.WriteLine($"Communicated Size = {communicatedSize}");
                //Debug.WriteLine($"Total Data  = {MeasureHolder.HostMeasureValueList.Last().AverageDataTotal}");


                ClearInformation();
            }

        }

        public void RequestCreated(MessageTypes type, double messageSize)
        {
            lock (_lock)
            {
                _currentRequests[type]++;
                communicatedSize += messageSize;
            }
        }

        private double CalculateNeededVolumeMax(List<HostLoadInfo> loads)
        {
            return loads.Select(x => x.Volume).Max();
        }

        private double CalculateNeededVolumeMin(List<HostLoadInfo> loads)
        {
            return loads.Select(x => x.Volume).Min();
        }

        private List<HostLoadInfo> GetLoadList(bool predicted)
        {
            List<HostLoadInfo> loads = new List<HostLoadInfo>();
            foreach (var machine in _machineTable.GetAllMachines().Skip(1).Where(x=>x.MachineId<int.MaxValue))
            {
                if (predicted)
                {
                    loads.Add((machine as HostMachine).GetPredictedHostLoadInfo());
                }
                else
                {
                    loads.Add((machine as HostMachine).GetNeededHostLoadInfo());
                    machine.CommunicationModule.ResetDataSize();
                }
            }
            return loads;
        }

        private int CalculateSlaViolations()
        {
            int slaViolations = 0;
            foreach (var machine in _machineTable.GetAllMachines().Skip(1).Where(x => x.MachineId < int.MaxValue))
            {
                slaViolations+=(machine as HostMachine).CalculateSlaViolations();
            }
            return slaViolations;
        }

        private void ClearInformation()
        {
            lock (_lock)
            {
                foreach (var t in Enum.GetValues(typeof(MessageTypes)).Cast<MessageTypes>())
                {
                    _currentRequests[t] = 0;
                }
                communicatedSize = 0;
            }
            
        }

        private double CalculateVolumeAverage(List<HostLoadInfo> loads )
        {
            List<double> a = new List<double>();
            foreach (var l in loads)
            {
                a.Add(l.Volume);
            }
            return a.Average();
        }
        /// <summary>
        /// All candidates are computed the larger one is selected
        /// the OS Load is ignored
        /// </summary>
        /// <returns></returns>
        private int CalculateIdealHostsCount(List<HostLoadInfo> loads )
        {
            Load load = new Load(0,0,0);
            foreach (var l in loads)
            {
                load += l.CurrentLoad;
            }
            var cpu = Math.Ceiling(load.CpuLoad / (Global.DataCenterHostConfiguration.CpuLoad * Global.MaxUtilization));
            var memory = Math.Ceiling(load.MemorySize / (Global.DataCenterHostConfiguration.MemorySize * Global.MaxUtilization));
            var io = Math.Ceiling(load.IoSecond / (Global.DataCenterHostConfiguration.IoSecond * Global.MaxUtilization));
            var max = cpu;
            if (max < memory)
            {
                max = memory;
            }
            if (max < io)
            {
                max = io;
            }
            return (int)max;
        }
        private void CalculateOutOfBoundHosts(out int u,out int o,out int n, out int e)
        {
            u = 0;
            o = 0;
            n = 0;
            foreach (var l in _machineTable.GetAllMachines().Skip(1).Where(x => x.MachineId < int.MaxValue))
            {
                var host = l as HostMachine;
                var masterUtil = _utilizationTable.GetUtilization(host.MachineId);
                var Volume = host.GetNeededHostLoadInfo().Volume;
                if (Volume < host.MinUtilization)
                {
                    u++;
                }
                else if (Volume > host.MaxUtilization)
                {
                    o++;
                }
                else
                {
                    n++;
                }
            }
            //int MU = _utilizationTable.GetCandidateHosts(UtilizationStates.UnderUtilization,0).Count;
            //int MO = _utilizationTable.GetCandidateHosts(UtilizationStates.OverUtilization, 0).Count;
            e = _utilizationTable.GetCandidateHosts(UtilizationStates.Evacuating, 0).Count;
            //int RN = _utilizationTable.GetCandidateHosts(UtilizationStates.Normal, 0).Count;

            //int f = 0;
        }

        private void CollectMigrationCounts()
        {
            lock (_lock)
            {
                Dictionary<int, ContainerMeasureValue> finalDictionary 
                    = new Dictionary<int, ContainerMeasureValue>();
                foreach (var machine in _machineTable.GetAllMachines().Skip(1).Where(x => x.MachineId < int.MaxValue))
                {
                    var host = machine as HostMachine;
                    foreach (var conDictionary in host.CollectMigrationCounts())
                    {
                        finalDictionary.Add(conDictionary.Key, conDictionary.Value);
                    }
                }
                var s = finalDictionary.OrderBy(x => x.Key).ToArray();
                finalDictionary.Clear();
                foreach (var item in s)
                {
                    finalDictionary.Add(item.Key, item.Value);
                }
                MeasureHolder.ContainerMigrationCount = finalDictionary;
            }
        }

        private void CollectImagePull()
        {
            lock (_lock)
            {
                if(MeasureHolder.ContainerType == ContainersType.D) {
                    var dockerRegistry = 
                        _machineTable.GetMachineById(int.MaxValue) as DockerRegistryMachine;
                    MeasureHolder.PullsPerImage = dockerRegistry.GetPullsPerImage();
                }
                
            }
        }
    }

}
