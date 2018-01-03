using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataCenter.Images
{
    public class Image
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int? BaseImage { get; set; }

        public Image(int id, string name, int? baseImage = null)
        {
            Id = id;
            Name = name;
            this.BaseImage = baseImage;
        }
    }
}
