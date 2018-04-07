using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.DataCenter.Core;

namespace Simulation.DataCenter.Containers
{
    public class Container : Component
    {
        object lck = new object();

        private CheckpointAndResotoreCalculator _calculator;
        public int ContainerId { get; }
        private LoadPrediction CurrentLoadPrediction { get; set; }
        private Load NeededLoad { get; set; }
        public override bool Started { get; set; } = false;
        private Load LastPredictedLoad { get; set; }
        private Load OldLoad { get; set; }
        private Load OlderLoad { get; set; }
        public int MigrationCount { get; private set; } = 0;
        public double DownTime { get; private set; } = 0;
        public ContainersType ContainerType { get; protected set; }

        public Container(int containerId, Load containerLoad, LoadPrediction currentLoadPrediction)
        {
            _calculator = new CheckpointAndResotoreCalculator();
            ContainerId = containerId;
            CurrentLoadPrediction = currentLoadPrediction;
            //ImageId = imageId;
            //MaxLoad = maxLoad;
            NeededLoad = new Load(containerLoad);
            Started = true;
            StartPrediction();
            ContainerType = ContainersType.N;
        }
        private void StartPrediction()
        {
            LastPredictedLoad = new Load(NeededLoad);
            OldLoad = new Load(NeededLoad);
            OlderLoad = new Load(NeededLoad);
            if (CurrentLoadPrediction == LoadPrediction.None)
            {
            }
            else if (CurrentLoadPrediction == LoadPrediction.Ewma)
            {
                Task predictionTask = new Task(async () =>
                {
                    while (Started)
                    {
                        await Task.Delay(Global.CheckRate);
                        var newLast = NeededLoad * Global.Alpha + LastPredictedLoad * (1 - Global.Alpha);
                        lock (lck)
                        {
                            LastPredictedLoad = newLast;
                        }
                    }
                });
                predictionTask.Start();
            }
            else if (CurrentLoadPrediction == LoadPrediction.Arma)
            {
                Task predictionTask = new Task(async () =>
                {
                    while (Started)
                    {
                        await Task.Delay(Global.CheckRate);

                        var newLast = NeededLoad * Global.Beta
                                      + OldLoad * Global.Gamma
                                      + OlderLoad * (1 - (Global.Beta + Global.Gamma));
                        lock (lck)
                        {
                            LastPredictedLoad = newLast;
                            OlderLoad = new Load(OldLoad);
                            OldLoad = new Load(NeededLoad);
                        }
                    }
                });
                predictionTask.Start();
            }
            else if (CurrentLoadPrediction == LoadPrediction.LinReg)
            {
                Task predictionTask = new Task(async () =>
                {
                    List<Load> oldLoads = new List<Load>();

                    while (Started)
                    {
                        await Task.Delay(Global.CheckRate/2);

                        var realLoad = new Load(NeededLoad);
                        oldLoads.Add(realLoad);
                        if (oldLoads.Count > Global.LinerRegWindow)
                        {
                            oldLoads.RemoveAt(0);
                        }
                        var predictor = new SimpleLinerRegression(oldLoads);
                        var newLast = predictor.TimePredict(Global.PredictionTime);
                        lock (lck)
                        {
                            LastPredictedLoad = newLast;
                        }
                    }
                });
                predictionTask.Start();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public void StopContainer()
        {
            Started = false;
        }

        public void ChangeLoad(LoadChangeAction changeAction)
        {
            var finalLoad = LoadGenerator.GetUpdatedContainerLoad(this.ContainerId, changeAction, NeededLoad);

            switch (Global.LoadChangeStrategy)
            {
                case LoadChangeStrategy.Force:
                    NeededLoad = finalLoad;
                    break;
                case LoadChangeStrategy.Incremental:
                    Task t = new Task(async () =>
                    {
                        var initalLoad = NeededLoad;
                        int steps = Global.Steps;
                        for (int i = 1; i <= steps; i++)
                        {
                            var newCurrent = (finalLoad - initalLoad) * i / steps + initalLoad;
                            lock (lck)
                            {
                                NeededLoad = newCurrent;
                            }
                            await Task.Delay(10 * Global.Second);
                        }
                    });
                    t.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected int CalculateMigrationCost()
        {
            return _calculator.GetCheckpointTime(NeededLoad.MemorySize) +
                   _calculator.GetRestorationTime(NeededLoad.MemorySize);
        }
        public ContainerLoadInfo GetContainerNeededLoadInfo()
        {
            lock (lck)
            {
                return CarveContainerLoadInfo(NeededLoad);
            }
        }
        public ContainerLoadInfo GetContainerPredictedLoadInfo()
        {
            //lock (lck)
            {
                Load finalLoad;
                switch (CurrentLoadPrediction)
                {
                    case LoadPrediction.None:
                        finalLoad = new Load(NeededLoad);
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

                return CarveContainerLoadInfo(finalLoad);
            }
        }

        protected virtual ContainerLoadInfo CarveContainerLoadInfo(Load load)
        {
            return new ContainerLoadInfo(this.ContainerId, 0, this.MigrationCount, CalculateMigrationCost(), new Load(load));
        }
        /// <summary>
        /// Should Simulate Correct checkpoint Time
        /// </summary>
        public async void Checkpoint(int hostId)
        {
            //lock (lck)
            {
                Console.WriteLine($"- Checkpoint Container #{ContainerId} from {hostId}");
                MigrationCount++;
                Started = false;
                int ctime = _calculator.GetCheckpointTime(this.NeededLoad.MemorySize);
                DownTime += 1.0 * ctime / Global.Second;
                await Task.Delay(ctime);
            }

        }
        /// <summary>
        /// Should Simulate Correct restore Time
        /// </summary>
        public async void Restore(int hostId)
        {
            //lock (lck)
            {
                Console.WriteLine($"+ Restore Container #{ContainerId} in {hostId}");

                int rTime = _calculator.GetRestorationTime(this.NeededLoad.MemorySize);
                DownTime += 1.0 * rTime / Global.Second;
                DownTime += Global.GetNetworkDelay(this.NeededLoad.MemorySize);
                await Task.Delay(rTime);
                Started = true;
                StartPrediction();
            }
        }

        /// <summary>
        /// CPU Violation only is accounted For
        /// </summary>
        /// <param name="loadDifference"></param>
        /// <param name="totalLoad"></param>
        /// <returns></returns>
        public bool IsViolated(Load loadDifference, Load totalLoad)
        {
            lock (lck)
            {
                double myPercent = NeededLoad.CpuLoad / totalLoad.CpuLoad;
                var currentLoad = NeededLoad - loadDifference * myPercent;
                myPercent = currentLoad.CpuLoad / NeededLoad.CpuLoad;
                if (myPercent < 0.95)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
