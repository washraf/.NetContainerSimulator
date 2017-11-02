using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataCenter.InformationModules
{
    public class ContainerRegistryTable
    {
        public Dictionary<int, Image> dictionary;

        public static Dictionary<int,Image> FillContainerRegistry()
        {
            return new Dictionary<int, Image>();
        }

        public ContainerRegistryTable()
        {
            dictionary = FillContainerRegistry();
        }

        public List<int> GetImageTree(int imageId)
        {
            List<int> list = new List<int>();
            if (dictionary.ContainsKey(imageId))
            {
                var image = dictionary[imageId];
                list.Add(imageId);
                while(image.Base!=null && dictionary.ContainsKey(image.Base.Id))
                {
                    image = dictionary[image.Id];
                    image = image.Base;
                    list.Add(image.Id);
                }
                return list;
            }
            else
                return null;
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
