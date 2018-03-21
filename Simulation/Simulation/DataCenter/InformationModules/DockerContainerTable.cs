using System;
using Simulation.DataCenter.Containers;

namespace Simulation.DataCenter.InformationModules
{
    public class DockerContainerTable : ContainerTable
    {
        private ImageManager ImageManager { get; }

        public DockerContainerTable(int machineId, ImageManager imageManager) : base(machineId)
        {
            ImageManager = imageManager;
        }
        public override void AddContainer(int containerId, Container container)
        {
            lock (_lock)
            {
                var dockerCon = container as DockerContainer;
                ImageManager.LoadImage(dockerCon.ImageId);
                ContainersTable.Add(containerId, container);
            }
        }
    }
}
