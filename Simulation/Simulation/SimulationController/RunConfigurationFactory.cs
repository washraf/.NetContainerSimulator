using Simulation.Configuration;
using System;
using System.Collections.Generic;

namespace Simulation.SimulationController
{
    public class RunConfigurationFactory
    {
        /// <summary>
        /// 50 => Burst
        /// 70 => Drain
        /// </summary>
        /// <returns></returns>
        public static List<RunConfiguration> GetConfigurations()
        {
            var Trials = new List<RunConfiguration>();
            var sizes = new List<SimulationSize>()
                    {
                        //SimulationSize.Five,
                        //SimulationSize.Ten,
                        SimulationSize.Twenty,
                        SimulationSize.Fifty,
                        //SimulationSize.Hundred,
                        //SimulationSize.TwoHundred
                    };
            var tested = new List<TestedHosts>()
            {
                //TestedHosts.Ten,
                //TestedHosts.Twenty,
                TestedHosts.Infinity,
            };

            foreach (var size in sizes)
            {
                foreach (var t in tested)
                {
                    Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.Burst,
                        LoadPrediction.None,
                        Strategies.Proposed2018,
                        SchedulingAlgorithm.FF,
                        t,ContainersType.D,0));
                    Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Drain,
                        LoadPrediction.None,
                        Strategies.Proposed2018,
                        SchedulingAlgorithm.FF,
                        t, ContainersType.D, 0));
                }
            }
            //Trials.AddRange(GetNoneConfigurations());
            return Trials;
        }
        public static List<RunConfiguration> GetNoneConfigurations()
        {
            var Trials = new List<RunConfiguration>();
            var sizes = new List<SimulationSize>()
                    {
                        //SimulationSize.Five,
                        //SimulationSize.Ten,
                        //SimulationSize.Twenty,
                        //SimulationSize.Fifty,
                        //SimulationSize.Hundred,
                        SimulationSize.TwoHundred
                    };
            var tested = new List<TestedHosts>()
            {
                TestedHosts.Ten,
                TestedHosts.Twenty,
                //TestedHosts.Infinity,
            };
            var schedulings = new List<SchedulingAlgorithm>()
            {
                //SchedulingAlgorithm.FF,
                //SchedulingAlgorithm.LFull,
                SchedulingAlgorithm.MFull,
            };
            foreach (var size in sizes)
            {
                foreach (var scheduling in schedulings)
                {
                    foreach (var t in tested)
                    {
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.None,
                        LoadPrediction.None,
                        Strategies.Proposed2018,
                        scheduling,
                        t, ContainersType.D,0));
                    }
                }
               
            }
            return Trials;
        }

        public static List<RunConfiguration> GetOldConfiguraton()
        {
            var Trials = new List<RunConfiguration>();
            var sizes = new List<SimulationSize>()
                    {
                        SimulationSize.Twenty,
                        SimulationSize.Fifty,
                        SimulationSize.Hundred,
                        SimulationSize.TwoHundred,
                    };
            var strategies = new List<Strategies>()
            {
                Strategies.WAshraf2017,
                Strategies.ForsmanPull,
                Strategies.ForsmanPush,
                Strategies.Zhao,
            };

            foreach (var size in sizes)
            {
                foreach (var strategy in strategies)
                {
                    for (int i = 7; i <= Global.NoOfTrials; i++)
                    {
                        LoadPrediction prediction;
                        switch (strategy)
                        {
                            case Strategies.WAshraf2017:
                                prediction = LoadPrediction.Arma;
                                break;
                            case Strategies.Zhao:
                                prediction = LoadPrediction.None;
                                break;
                            case Strategies.ForsmanPush:
                            case Strategies.ForsmanPull:
                                prediction = LoadPrediction.Ewma;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        //Basic
                        //Trials.Add(new RunConfiguration(size,
                        //StartUtilizationPercent.Fifty,
                        //LoadChangeAction.Burst,
                        //prediction,
                        //strategy,
                        //SchedulingAlgorithm.FF,
                        //TestedHosts.Infinity,
                        //ContainersType.N,
                        //i));
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Drain,
                        prediction,
                        strategy,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Infinity,
                        ContainersType.N,
                        i));

                        //Reverse
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.Drain,
                        prediction,
                        strategy,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Infinity,
                        ContainersType.N,
                        i));
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Burst,
                        prediction,
                        strategy,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Infinity,
                        ContainersType.N,
                        i));

                        //Opposite
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.Opposite,
                        prediction,
                        strategy,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Infinity,
                        ContainersType.N,
                        i));
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Opposite,
                        prediction,
                        strategy,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Infinity,
                        ContainersType.N,
                        i));

                    }

                }
            }
            return Trials;
        }
    }

}
