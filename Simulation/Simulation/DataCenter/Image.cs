using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataCenter
{
    public class Image
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public Image Base { get; set; }

    }
}
