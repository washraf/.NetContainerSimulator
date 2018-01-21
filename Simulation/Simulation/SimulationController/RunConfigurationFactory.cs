using Simulation.Configuration;
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
                        SimulationSize.Hundred,
                        SimulationSize.TwoHundred
                    };
            foreach (var size in sizes)
            {
                Trials.Add(new RunConfiguration(size,
                    StartUtilizationPercent.Fifty,
                    LoadChangeAction.Burst,
                    LoadPrediction.None,
                    Strategies.Proposed2018,
                    SchedulingAlgorithm.FF,
                    TestedHosts.One));
                Trials.Add(new RunConfiguration(size,
                    StartUtilizationPercent.Fifty,
                    LoadChangeAction.Burst,
                    LoadPrediction.None,
                    Strategies.Proposed2018,
                    SchedulingAlgorithm.FF,
                    TestedHosts.Twenty));
                Trials.Add(new RunConfiguration(size,
                    StartUtilizationPercent.Seventy,
                    LoadChangeAction.Drain,
                    LoadPrediction.None,
                    Strategies.Proposed2018,
                    SchedulingAlgorithm.FF,
                    TestedHosts.One));
                Trials.Add(new RunConfiguration(size,
                    StartUtilizationPercent.Seventy,
                    LoadChangeAction.Drain,
                    LoadPrediction.None,
                    Strategies.Proposed2018,
                    SchedulingAlgorithm.FF,
                    TestedHosts.Twenty));
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
                        SimulationSize.Fifty,
                        SimulationSize.Hundred,
                        SimulationSize.TwoHundred
                    };
            foreach (var size in sizes)
            {
                Trials.Add(new RunConfiguration(size,
                    StartUtilizationPercent.Fifty,
                    LoadChangeAction.None,
                    LoadPrediction.None,
                    Strategies.Proposed2018,
                    SchedulingAlgorithm.FF,
                    TestedHosts.Twenty));
            }
            return Trials;
        }
    }
    
}
