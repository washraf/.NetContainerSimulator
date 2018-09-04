using Simulation.Configuration;
using System;
using System.IO;
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
                //SimulationSize.TwoHundred,
                SimulationSize.Hundred,
                SimulationSize.Fifty,
                SimulationSize.Twenty,
            };
            var TestedPercent = new List<TestedHosts>()
            {
                //TestedHosts.TwentyFive,
                //TestedHosts.Fifty,
                //TestedHosts.SeventyFive,
                TestedHosts.Hundred,
            };
            var auctionTypes = new List<AuctionTypes>()
            {
                AuctionTypes.LeastFull,
                AuctionTypes.MostFull,
                AuctionTypes.Random,
                AuctionTypes.LeastPulls
            };
            var lastindex = getLastIndex();
            for (int tId = 1+lastindex; tId <= Global.NoOfTrials+lastindex; tId++)
            {
                foreach (var size in sizes)
                {
                    foreach (var auction in auctionTypes)
                    {
                        foreach (var testedPercent in TestedPercent)
                        {
                            Trials.Add(new RunConfiguration(size,
                                StartUtilizationPercent.Fifty,
                                LoadChangeAction.Burst,
                                LoadPrediction.Arma,
                                Strategies.Proposed2018,
                                auction,
                                auction,
                                SchedulingAlgorithm.FF,
                                testedPercent,
                                ContainersType.D,
                                true,
                                tId));
                            Trials.Add(new RunConfiguration(size,
                                StartUtilizationPercent.Seventy,
                                LoadChangeAction.Drain,
                                LoadPrediction.Arma,
                                Strategies.Proposed2018,
                                auction,
                                auction,
                                SchedulingAlgorithm.FF,
                                testedPercent,
                                ContainersType.D,
                                true,
                                tId));
                        }
                    }
                }
                Trials.AddRange(GetOtherConfiguration(tId));
            }
            return Trials;
        }

        private static int getLastIndex()
        {
            try
            {
                StreamReader r = new StreamReader("D:/Last.txt");
                var x = r.ReadToEnd();
                var i = int.Parse(x);
                return i;
            }
            catch
            {
                return 0;
            }
        }

        public static List<RunConfiguration> GetOtherConfiguration(int trialId)
        {
            var Trials = new List<RunConfiguration>();
            var Sizes = new List<SimulationSize>()
            {
                //SimulationSize.TwoHundred,
                SimulationSize.Hundred,
                SimulationSize.Fifty,
                SimulationSize.Twenty,
            };
            var Algorithsms = new List<Strategies>()
            {
                Strategies.ForsmanPull,
                Strategies.ForsmanPush,
                Strategies.Zhao,
            };
            var prediction = LoadPrediction.None;

            foreach (var size in Sizes)
            {
                foreach (var alg in Algorithsms)
                {
                    if (alg == Strategies.ForsmanPush || alg == Strategies.ForsmanPull)
                        prediction = LoadPrediction.Ewma;
                    else
                        prediction = LoadPrediction.None;
                    Trials.Add(new RunConfiguration(size,
                                StartUtilizationPercent.Fifty,
                                LoadChangeAction.Burst,
                                prediction,
                                alg,
                                AuctionTypes.Ignore,
                                AuctionTypes.Ignore,
                                SchedulingAlgorithm.FF,
                                TestedHosts.Hundred,
                                ContainersType.D,
                                true,
                                trialId));
                    Trials.Add(new RunConfiguration(size,
                                StartUtilizationPercent.Seventy,
                                LoadChangeAction.Drain,
                                prediction,
                                alg,
                                AuctionTypes.Ignore,
                                AuctionTypes.Ignore,
                                SchedulingAlgorithm.FF,
                                TestedHosts.Hundred,
                                ContainersType.D,
                                true,
                                trialId));
                }

            }
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
                        SimulationSize.Hundred,
                        SimulationSize.TwoHundred
                    };
            var tested = new List<TestedHosts>()
            {
                //TestedHosts.Ten,
                //TestedHosts.Twenty,
                TestedHosts.Hundred,
            };
            var schedulings = new List<SchedulingAlgorithm>()
            {
                SchedulingAlgorithm.FF,
                //SchedulingAlgorithm.LFull,
               // SchedulingAlgorithm.MFull,
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
                        AuctionTypes.LeastFull,
                        AuctionTypes.MostFull,
                        scheduling,
                        t, ContainersType.D,
                        true,
                        0));
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
                    for (int i = 0; i <= Global.NoOfTrials; i++)
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
                        AuctionTypes.Ignore,
                        AuctionTypes.Ignore,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Hundred,
                        ContainersType.N,
                        false,
                        i));

                        //Reverse
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.Drain,
                        prediction,
                        strategy,
                        AuctionTypes.Ignore,
                        AuctionTypes.Ignore,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Hundred,
                        ContainersType.N,
                        false,
                        i));
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Burst,
                        prediction,
                        strategy,
                        AuctionTypes.Ignore,
                        AuctionTypes.Ignore,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Hundred,
                        ContainersType.N,
                        false,
                        i));

                        //Opposite
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Fifty,
                        LoadChangeAction.Opposite,
                        prediction,
                        strategy,
                        AuctionTypes.Ignore,
                        AuctionTypes.Ignore,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Hundred,
                        ContainersType.N,
                        false,
                        i));
                        Trials.Add(new RunConfiguration(size,
                        StartUtilizationPercent.Seventy,
                        LoadChangeAction.Opposite,
                        prediction,
                        strategy,
                        AuctionTypes.Ignore,
                        AuctionTypes.Ignore,
                        SchedulingAlgorithm.FF,
                        TestedHosts.Hundred,
                        ContainersType.N,
                        false,
                        i));

                    }

                }
            }
            return Trials;
        }
    }

}
