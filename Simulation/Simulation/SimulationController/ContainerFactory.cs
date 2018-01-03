using Simulation.DataCenter.Containers;
using Simulation.Loads;
using Simulation.Configuration;
using Simulation.LocationStrategies;
using Simulation.Helpers;
using System;

namespace Simulation.SimulationController
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
                int imgId = random.Next((int)_simulationSize * 3, (int)_simulationSize * 6);
                return new DockerContainer(conId, load, predictionStrategy, imgId);
            }
            else
            {
                return new Container(conId, load, predictionStrategy);
            }
        }
    }
}
