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
        public readonly Dictionary<int, Image> dictionary;
        public readonly Dictionary<int, int> pullsPerImage;
        public static Dictionary<int,Image> FillContainerRegistry(SimulationSize simulationSize)
        {
            var dic =  new Dictionary<int, Image>();
            int size = (int)simulationSize;
            for (int i = 0; i < size; i++)
            {
                dic.Add(i, new Image(i, $"Base {i}",100));

            }
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 2* size; i++)
            {
                var id = i+ size;
                var bimg = random.Next(0, 20);
                dic.Add(id, new Image(id, $"Level {id}", 100, bimg));

            }
            for (int i = 0; i < 3 * (int)simulationSize; i++)
            {
                var id = i + 3*size;
                var bimg = random.Next(20, 60);
                dic.Add(id, new Image(id, $"Final {id}", 100, bimg));

            }
            return dic;
        }

        public RegistryTable(SimulationSize simulationSize)
        {
            dictionary = FillContainerRegistry(simulationSize);
            pullsPerImage = new Dictionary<int, int>();
            foreach (var item in dictionary)
            {
                pullsPerImage.Add(item.Key, 0);
            }
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
                pullsPerImage[imageId]++;
                return dictionary[imageId];
            }
            else
                return null;
        }

        public Dictionary<int, int> GetPullsPerImage()
        {
            return pullsPerImage;
        }
    }
}
