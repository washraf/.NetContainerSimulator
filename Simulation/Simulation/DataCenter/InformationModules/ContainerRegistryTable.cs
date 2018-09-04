using Simulation.Configuration;
using Simulation.DataCenter.Images;
using Simulation.Helpers;
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
        /// <summary>
        /// The Images count is 15 * the simulation size
        /// The images is almost equals the number of containers
        /// </summary>
        /// <param name="simulationSize"></param>
        /// <returns></returns>
        public static Dictionary<int,Image> FillContainerRegistry(SimulationSize simulationSize)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            var dic =  new Dictionary<int, Image>();
            int size = (int)simulationSize;
            for (int i = 0; i < size; i++)
            {
                int imageSize = (int)random.NextGaussian(50, 20);
                dic.Add(i, new Image(i, $"Base {i}",imageSize));

            }
            for (int i = 0; i < 2* size; i++)
            {
                var id = i+ size;
                var bimg = random.Next(0, size);
                int imageSize = (int)random.NextGaussian(50, 20);
                dic.Add(id, new Image(id, $"Level {id}",imageSize , bimg));

            }
            for (int i = 0; i < 3 * size; i++)
            {
                var id = i + 3*size;
                var bimg = random.Next(size, 3*size);
                int imageSize = (int)random.NextGaussian(50, 20);
                dic.Add(id, new Image(id, $"Final {id}", imageSize, bimg));

            }

            for (int i = 0; i < 4 * size; i++)
            {
                var id = i + 6 * size;
                var bimg = random.Next(3*size, 6*size);
                int imageSize = (int)random.NextGaussian(50, 20);
                dic.Add(id, new Image(id, $"Final {id}", imageSize, bimg));
            }

            //for (int i = 0; i < 5 * size; i++)
            //{
            //    var id = i + 10 * size;
            //    var bimg = random.Next(6*size, 10 * size);
            //    int imageSize = (int)random.NextGaussian(50, 20);
            //    dic.Add(id, new Image(id, $"Final {id}", imageSize, bimg));
            //}

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

        internal List<Image> GetImageList(int imageId)
        {
            List<Image> list = new List<Image>();
            var image = dictionary[imageId];
            list.Add(image);
            while (image.BaseImage.HasValue)
            {
                image = dictionary[image.BaseImage.Value];
                list.Add(image);
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
