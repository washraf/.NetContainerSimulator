using Simulation.DataCenter.InformationModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataCenter
{
    public class ContainerImageRegistry
    {
        ContainerRegistryTable table;

        public NetworkSwitch NetworkSwitch { get; }

        public ContainerImageRegistry(NetworkSwitch networkSwitch)
        {
            table = new ContainerRegistryTable();
            NetworkSwitch = networkSwitch;
        }

        public List<int> GetImageTree(int imageId)
        {
            return table.GetImageTree(imageId);
        }

        public Image GetImage(int imageId)
        {
            return table.GetImage(imageId);
        }
    }
}
