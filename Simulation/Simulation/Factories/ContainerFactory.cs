using Simulation.DataCenter.Containers;
using Simulation.Loads;
using Simulation.Configuration;
using Simulation.LocationStrategies;
using Simulation.Helpers;
using System;

namespace Simulation.Factories
{
    public class ContainerFactory
    {
        private readonly SimulationSize _simulationSize;
        private readonly LoadPrediction predictionStrategy;
        Random random;
        public ContainerFactory(ContainersType containertype, SimulationSize simulationSize, LoadPrediction predictionStrategy)
        {
            Containertype = containertype;
            _simulationSize = simulationSize;
            this.predictionStrategy = predictionStrategy;
            random = new Random(Guid.NewGuid().GetHashCode());

        }

        public ContainersType Containertype { get; }

        public Container GetContainer(Load load)
        {
            var conId = RandomNumberGenerator.GetContainerRandomNumber();
            if (Containertype == ContainersType.D)
            {
                int imgId = random.Next((int)_simulationSize * 10, (int)_simulationSize * 15);
                return new DockerContainer(conId, load, predictionStrategy, imgId);
            }
            else
            {
                return new Container(conId, load, predictionStrategy);
            }
        }
    }
}
