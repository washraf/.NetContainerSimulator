using System;
using Simulation.DataCenter.Containers;
using Simulation.Configuration;
using System.Threading.Tasks;

namespace Simulation.DataCenter.InformationModules
{
    public class DockerContainerTable : ContainerTable
    {
        private ImageManager ImageManager { get; }

        public DockerContainerTable(int machineId, ImageManager imageManager) : base(machineId)
        {
            ImageManager = imageManager;
            ContainerType = ContainersType.D;
        }
        public override void AddContainer(int containerId, Container container)
        {
            lock (_lock)
            {
                var dockerCon = container as DockerContainer;
                if (!ImageManager.ContainsImage(dockerCon.ImageId))
                {
                    throw new Exception("How Come");
                }
                ContainersTable.Add(containerId, container);
            }
        }
        public async Task LoadImage(int imageId)
        {
            await ImageManager.LoadImage(imageId);
        }

        public int GetNumberOfPulls(int imageId)
        {
            if (ImageManager.ContainsImage(imageId))
                return 0;
            else
            {
                var result = 0;
                var task = ImageManager.GetImageTree(imageId);
                task.Wait();
                var ids = task.Result;
                foreach (var id in ids)
                {
                    if (!ImageManager.ContainsImage(id))
                        result++;
                }
                return result;
            }
        }
    }
}
