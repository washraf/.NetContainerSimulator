using Simulation.Configuration;
using Simulation.DataCenter.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataCenter.InformationModules
{
    public class RegistryTable
    {
        public Dictionary<int, Image> dictionary;

        public static Dictionary<int,Image> FillContainerRegistry(SimulationSize simulationSize)
        {
            var dic =  new Dictionary<int, Image>();

            for (int i = 0; i < (int)simulationSize; i++)
            {
                dic.Add(i, new Image(i, $"Base {i}"));

            }
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 2* (int)simulationSize; i++)
            {
                var id = i+ 20;
                var bimg = random.Next(0, 20);
                dic.Add(id, new Image(id, $"Level {id}",bimg));

            }
            for (int i = 0; i < 3 * (int)simulationSize; i++)
            {
                var id = i + 60;
                var bimg = random.Next(20, 60);
                dic.Add(id, new Image(id, $"Final {id}", bimg));

            }
            return dic;
        }

        public RegistryTable(SimulationSize simulationSize)
        {
            dictionary = FillContainerRegistry(simulationSize);
        }

        public List<int> GetImageTree(int imageId)
        {
            List<int> list = new List<int>();
            var image = dictionary[imageId];
            list.Add(imageId);
            while (image.BaseImage.HasValue) //&& dictionary.ContainsKey(image.Base.Id)
            {
                //image = dictionary[image.Id];
                image = dictionary[image.BaseImage.Value];
                list.Add(image.Id);
            }
            list.Reverse();
            return list;
        }

        public Image GetImage(int imageId)
        {
            if (dictionary.ContainsKey(imageId))
            {
                return dictionary[imageId];
            }
            else
                return null;
        }
    }
}
