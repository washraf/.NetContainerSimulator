using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.Messages;

namespace Simulation.Modules.LoadManagement
{
    public class HostLoadManager : ILoadManager
    {
        private readonly object _lock = new object();
        private readonly Load _maxLoad;
        private readonly NetworkInterfaceCard _networkCard;
        private readonly Load _osLoad;
        private readonly ContainerTable _containerTable;
        private UtilizationStates LastState { get; set; } = UtilizationStates.Normal;

        public int MachineId { get; set; }
        public LoadPrediction CurrentLoadPrediction { get; set; }
        public HostLoadManager(int machineId, Load maxLoad, LoadPrediction loadPrediction, NetworkInterfaceCard networkCard, ContainerTable containerTable)
        {
            _maxLoad = maxLoad;
            _networkCard = networkCard;
            _osLoad = new Load(maxLoad.CpuLoad * 0.01, 100, 0);
            _containerTable = containerTable;
            MachineId = machineId;
            CurrentLoadPrediction = loadPrediction;
        }

        #region --Load Calculation--
        
        private Load CalculateNeededTotalLoad()
        {
            var totalLoad = new Load(_osLoad);
            totalLoad += _containerTable.GetContainersTotalNeededLoad();
            return new Load(totalLoad);
        }

        private Load CalculatePredictedTotalLoad()
        {
            switch (Global.PredictionSource)
            {
                case PredictionSource.Container:
                    var totalLoad = new Load(_osLoad);
                    totalLoad += _containerTable.GetContainersTotalPredictedLoad();

                    return new Load(totalLoad);

                case PredictionSource.Host:
                    Load finalLoad;
                    switch (CurrentLoadPrediction)
                    {
                        case LoadPrediction.None:
                            finalLoad = CalculateNeededTotalLoad();
                            break;
                        case LoadPrediction.Ewma:
                            finalLoad = LastPredictedLoad;
                            break;
                        case LoadPrediction.Arma:
                            finalLoad = LastPredictedLoad;
                            break;
                        case LoadPrediction.LinReg:
                            finalLoad = LastPredictedLoad;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (finalLoad == null || finalLoad.CpuLoad<0)
                    {
                        throw new NotImplementedException("Final Load");
                    }
                    return new Load(finalLoad);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Load LastPredictedLoad { get; set; }
        private Load OldLoad { get; set; }
        private Load OlderLoad { get; set; }

        public void Start()
        {
            StartLoadPrediction();
            StartSLAViolationCounter();
        }
        Dictionary<int,int> violatedContainers = new Dictionary<int, int>(); 
        private void StartSLAViolationCounter()
        {
            Task t = new Task( async () =>
            {
                while (Started)
                {
                    await Task.Delay(Global.Second);
                    var needed = GetNeededHostLoadInfo();
                    var v = needed.Volume;
                    if (v > 1)
                    {
                        var  vioCon = _containerTable.CalculateSlaViolations(needed.CurrentLoad - _maxLoad);
                        foreach (var conId in vioCon)
                        {
                            lock (_lock)
                            {
                                if (!violatedContainers.ContainsKey(conId))
                                {
                                    violatedContainers.Add(conId, 1);
                                }
                                else
                                {
                                    violatedContainers[conId]++;
                                }
                            }
                        }
                    }
                }
            });
            t.Start();
        }

        public int CalculateSlaViolations()
        {
            lock (_lock)
            {
                var count = violatedContainers.Count(x => x.Value > 1);
                violatedContainers.Clear();
                return count;
            }
        }

        private void StartLoadPrediction()
        {
            LastPredictedLoad = CalculateNeededTotalLoad();
            OldLoad = new Load(LastPredictedLoad);
            OlderLoad = new Load(LastPredictedLoad);
            if (Global.PredictionSource == PredictionSource.Host)
            {
                if (CurrentLoadPrediction == LoadPrediction.None) { }
                else if (CurrentLoadPrediction == LoadPrediction.Ewma)
                {
                    Task predictionThread = new Task(async () =>
                    {
                        while (Started)
                        {
                            await Task.Delay(Global.Second);
                            
                                var realLoad = CalculateNeededTotalLoad();
                                var newPredicted = realLoad*Global.Alpha + LastPredictedLoad*(1 - Global.Alpha);
                            lock (_lock)
                            {
                                LastPredictedLoad = new Load(newPredicted);
                            }
                        }
                    });
                    predictionThread.Start();
                }
                else if (CurrentLoadPrediction == LoadPrediction.Arma)
                {
                    Task predictionThread = new Task(async () =>
                    {
                        while (Started)
                        {
                            await Task.Delay(Global.Second);
                            
                                var realLoad = CalculateNeededTotalLoad();

                                var newPredicted =
                                    realLoad*Global.Beta
                                    + OldLoad*Global.Gamma
                                    + OlderLoad*(1 - (Global.Beta + Global.Gamma));
                            lock (_lock)
                            {
                                LastPredictedLoad = newPredicted;
                                OlderLoad = new Load(OldLoad);
                                OldLoad = new Load(realLoad);
                            }
                        }
                    });
                    predictionThread.Start();
                }
                else if (CurrentLoadPrediction == LoadPrediction.LinReg)
                {
                    Task predictionThread = new Task(async () =>
                    {
                        List<Load> oldLoads = new List<Load>();
                        while (Started)
                        {
                            await Task.Delay(Global.Second);
                            var realLoad = CalculateNeededTotalLoad();
                            oldLoads.Add(realLoad);
                            if (oldLoads.Count > Global.LinerRegWindow)
                            {
                                oldLoads.RemoveAt(0);
                            }
                            var predictor = new SimpleLinerRegression(oldLoads);
                            var newPredicted = predictor.TimePredict(Global.LinerRegWindow + Global.PredictionTime);
                            lock (_lock)
                            {
                                if(newPredicted.CpuLoad<0)
                                    throw new NotImplementedException("Host Load manager");
                                LastPredictedLoad = newPredicted;
                            }
                        }
                    });
                    predictionThread.Start();
                }
                else
                {
                    throw new NotImplementedException("How Come exception");
                }
            }

        }

        public HostLoadInfo GetNeededHostLoadInfo()
        {
            var totalLoad = CalculateNeededTotalLoad();
            var load = CarveHostLoadInfo(totalLoad);
            return load;
        }
        public HostLoadInfo GetPredictedHostLoadInfo()
        {
            var totalLoad = CalculatePredictedTotalLoad();
            var load = CarveHostLoadInfo(totalLoad);
            return load;
        }

        public bool CanITakeLoad(ContainerLoadInfo containerLoadInfo)
        {
            var totalLoad = CalculatePredictedTotalLoad();
            return totalLoad.MemorySize < _maxLoad.MemorySize;
        }

        /// <summary>
        /// Test Can Take Load and return load after new load
        /// </summary>
        /// <param name="containerLoadInfo"></param>
        /// <returns></returns>
        public HostLoadInfo GetHostLoadInfoAfterContainer(ContainerLoadInfo containerLoadInfo)
        {
            var totalLoad = CalculatePredictedTotalLoad();
            totalLoad += containerLoadInfo.CurrentLoad;
            var load = CarveHostLoadInfo(totalLoad);
            return load;
        }

        public HostLoadInfo GetHostLoadInfoAWithoutContainer(ContainerLoadInfo containerLoadInfo)
        {
            var totalLoad = CalculatePredictedTotalLoad();
            totalLoad -= containerLoadInfo.CurrentLoad;
            var load = CarveHostLoadInfo(totalLoad);
            return load;
        }

        private HostLoadInfo CarveHostLoadInfo(Load totalLoad)
        {
            if (totalLoad.CpuLoad < 0)
            {
                //throw new NotImplementedException("Carve Load host info");
            }
            var CPUUtil = totalLoad.CpuLoad/_maxLoad.CpuLoad;
            var IOUtil = totalLoad.IoSecond/_maxLoad.IoSecond;
            var MemoryUtil = totalLoad.MemorySize/_maxLoad.MemorySize;
            var load = new HostLoadInfo(this.MachineId, totalLoad,_containerTable.GetContainersCount(), CPUUtil, MemoryUtil, IOUtil);
            return load;
        }


        #endregion

        public bool Started { get; set; }
        int f = 0;
        public UtilizationStates CheckSystemState(bool act,double min,double max)
        {
            var loadInfo = GetPredictedHostLoadInfo();
            var hoststate = loadInfo.CalculateTotalUtilizationState(min,max);
            if ((act && hoststate != LastState)|| f==0)
            {
                ReportUtilizationStateChange(hoststate, loadInfo.CPUUtil);
                LastState = hoststate;
                f++;
            }
            return hoststate;
        }



        private void ReportUtilizationStateChange(UtilizationStates hoststate, double util)
        {
            HostStateChange change = new HostStateChange(0, this.MachineId, hoststate, util);
            _networkCard.SendMessage(change);
        }
    }
}