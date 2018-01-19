using Simulation.Configuration;

namespace Simulation.SimulationController
{
    public class RunConfiguration
    {
        public RunConfiguration(SimulationSize simulationSize, 
            StartUtilizationPercent startPercent,
            LoadChangeAction changeAction,
            LoadPrediction loadPrediction,
            Strategies strategy,
            TestedHosts testedHosts = TestedHosts.One, ContainersType containersType = ContainersType.D)
        {
            LoadPrediction = loadPrediction;
            SimulationSize = simulationSize;
            StartPercent = startPercent;
            Strategy = strategy;
            ChangeAction = changeAction;
            TestedHosts = testedHosts;
            ContainersType = containersType;
        }

        public LoadPrediction LoadPrediction { get; }
        public SimulationSize SimulationSize { get; }
        public StartUtilizationPercent StartPercent { get; }
        public Strategies Strategy { get; }
        public LoadChangeAction ChangeAction { get; }
        public TestedHosts TestedHosts { get; }
        public ContainersType ContainersType { get; }
    }    
}
