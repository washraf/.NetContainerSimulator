using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation.Loads
{
    public class Load
    {
        public Load(double cpu, double memory,double io)
        {
            if (double.IsNaN(cpu))
            {
                throw new NotImplementedException("Load ");
                
            }
            CpuLoad = cpu;
            MemorySize = memory;
            IoSecond = io;
        }
        public Load(Load load)
        {
            CpuLoad = load.CpuLoad;
            MemorySize = load.MemorySize;
            IoSecond = load.IoSecond;
        }
        /// <summary>
        /// Unit of 1
        /// </summary>
        public double CpuLoad { get; }
        /// <summary>
        /// Unit of MB
        /// </summary>
        public double MemorySize { get; }
        /// <summary>
        /// Unit of MB/Sec
        /// </summary>
        public double IoSecond { get; }

        public static Load operator +(Load f, Load s)
        {
            return new Load(f.CpuLoad+s.CpuLoad,f.MemorySize+s.MemorySize,f.IoSecond+s.IoSecond);
        }

        public static Load operator -(Load f, Load s)
        {
            return new Load(f.CpuLoad - s.CpuLoad, f.MemorySize - s.MemorySize, f.IoSecond - s.IoSecond);
        }

        public static Load operator /(Load f, double s)
        {
            return new Load(f.CpuLoad/s,f.MemorySize/s,f.IoSecond/s);
        }

        public static Load operator *(Load f, double s)
        {
            return new Load(f.CpuLoad * s, f.MemorySize * s, f.IoSecond * s);
        }

        public override string ToString()
        {
            return $"{CpuLoad}, {MemorySize},{IoSecond}";
        }

    }
}
