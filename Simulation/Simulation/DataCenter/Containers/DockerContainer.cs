using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.Loads;

namespace Simulation.DataCenter.Containers
{
    public class DockerContainer : Container
    {
        public int ImageId { get; }

        public DockerContainer(int containerId, Load containerLoad, LoadPrediction currentLoadPrediction, int imageId) 
            : base(containerId, containerLoad, currentLoadPrediction)
        {
            ImageId = imageId;
            ContainerType = ContainersType.D;
        }

        protected override ContainerLoadInfo CarveContainerLoadInfo(Load load)
        {
            return new ContainerLoadInfo(this.ContainerId, ImageId, this.MigrationCount, CalculateMigrationCost(), new Load(load));
        }

    }
}
