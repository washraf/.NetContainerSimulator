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

namespace Simulation.Accounting
{
    public class AccountingModule:IAccountingModule
    {
        public AccountingModule(MachineTable machineTable)
        {
            ClearInformation();
            _machineTable = machineTable;

        }
        private object _lock = new object();

        //private Thread myThread = null;

        Dictionary<MessageTypes, int> _currentRequests = new Dictionary<MessageTypes, int>();
        public MeasureValueHolder MeasureHolder { get;} = new MeasureValueHolder(Global.CurrentStrategy,Global.SimulationSize, Global.StartUtilizationPercent, Global.ChangeAction, Global.LoadPrediction, Global.TestedItems);
       

        //public List<MeasuresValues> MeasuredValuesList { set; get; } = new List<MeasuresValues>();
        //private bool _started;
        private MachineTable _machineTable;

        //public void StartCounting()
        //{
        //    //lock (_lock)
        //    {
        //        //ClearInformation();
        //        //_started = true;
        //        //myThread = new Thread(Start);
        //        //myThread.Start();
        //    }
        //}

        public void StopCounting()
        {
            CollectMigrationCounts();
            //lock (_lock)
            //{
            //    _started = false;
            //}
        }
        //private void Start()
        //{
        //    while (_started)
        //    {
        //        CleanAndAdd();
        //        Thread.Sleep(Global.AccountTime);
        //    }
        //}

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
                int underUtilized, overUtilizedHosts;
                CalculateOutOfBoundHosts(out underUtilized,out overUtilizedHosts);
                int slaViolations = CalculateSlaViolations();
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
                    slaViolations,
                    loads.PowerConsumption(),
                    loads.StandardDeviation()
                    );
                ClearInformation();
                MeasureHolder.LoadMeasureValueList.Add(new LoadMeasureValue(loads));
                MeasureHolder.MeasuredValuesList.Add(mvalues);
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
            foreach (var machine in _machineTable.GetAllMachines().Skip(1))
            {
                if (predicted)
                {
                    loads.Add((machine as HostMachine).GetPredictedHostLoadInfo());
                }
                else
                {
                    loads.Add((machine as HostMachine).GetNeededHostLoadInfo());
                }
            }
            return loads;
        }

        private int CalculateSlaViolations()
        {
            int slaViolations = 0;
            foreach (var machine in _machineTable.GetAllMachines().Skip(1))
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
            }
            
        }

        public void RequestCreated(MessageTypes type)
        {
            lock (_lock)
            {
                _currentRequests[type] ++;
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
        private void CalculateOutOfBoundHosts(out int u,out int o)
        {

            int under = 0,over = 0;
            foreach (var l in _machineTable.GetAllMachines().Skip(1))
            {
                var host = l as HostMachine;
                if (host.GetNeededHostLoadInfo().Volume < host.MinUtilization)
                {
                    under++;
                }
                else if (host.GetNeededHostLoadInfo().Volume > host.MaxUtilization)
                {
                    over++;
                }
            }
            u = under;
            o = over;
        }

        private void CollectMigrationCounts()
        {
            lock (_lock)
            {
                Dictionary<int, ContainerMeasureValue> finalDictionary = new Dictionary<int, ContainerMeasureValue>();
                foreach (var machine in _machineTable.GetAllMachines().Skip(1))
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
    }
}
