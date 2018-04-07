using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.Measure
{
    public class HostMeasureValues
    {
        private HostMeasureValues()
        {

        }
        public HostMeasureValues(List<HostLoadInfo> loads)
        {
            foreach (var l in loads)
            {
                CurrentValues.Add(l.HostId, l);
            }
        }

        public HostMeasureValues(HostMeasureValues item)
        {
            foreach (var l in item.CurrentValues.Values)
            {
                CurrentValues.Add(l.HostId, l);
            }
        }

        /// <summary>
        /// Key => Host Id
        /// Value => Host Load Info for this id
        /// </summary>
        public Dictionary<int, HostLoadInfo> CurrentValues { set; get; } = new Dictionary<int, HostLoadInfo>();

        public static HostMeasureValues operator +(HostMeasureValues first, HostMeasureValues second)
        {
            var final = new HostMeasureValues();
            foreach (var l in first.CurrentValues.Values)
            {
                final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId, l.CurrentLoad, l.ContainersCount, l.CPUUtil, l.MemoryUtil, l.IOUtil, l.DataSizeOut, l.DataSizeIn));
            }
            foreach (var l in second.CurrentValues.Values)
            {
                if (final.CurrentValues.ContainsKey(l.HostId))
                {
                    final.CurrentValues[l.HostId] = new HostLoadInfo(l.HostId,
                        final.CurrentValues[l.HostId].CurrentLoad + l.CurrentLoad,
                        final.CurrentValues[l.HostId].ContainersCount + l.ContainersCount,
                        final.CurrentValues[l.HostId].CPUUtil + l.CPUUtil,
                        final.CurrentValues[l.HostId].MemoryUtil + l.MemoryUtil,
                        final.CurrentValues[l.HostId].IOUtil + l.IOUtil,
                        final.CurrentValues[l.HostId].DataSizeOut + l.DataSizeOut,
                        final.CurrentValues[l.HostId].DataSizeIn + l.DataSizeIn
                        );
                }
                else
                {
                    final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId, l.CurrentLoad, l.ContainersCount, l.CPUUtil, l.MemoryUtil, l.IOUtil, l.DataSizeOut, l.DataSizeIn));
                }
            }
            return final;
        }

        public static HostMeasureValues operator /(HostMeasureValues first, int c)
        {
            var final = new HostMeasureValues();
            foreach (var l in first.CurrentValues.Values)
            {
                final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId, l.CurrentLoad / c, l.ContainersCount / c, l.CPUUtil / c, l.MemoryUtil / c, l.IOUtil / c, l.DataSizeOut / c, l.DataSizeIn / c));
            }
            return final;
        }


        //Aggregates
        public double StandardDeviation
        {
            get { return CurrentValues.Values.ToList().StandardDeviation(); }
        }

        public double Containers
        {
            get
            {
                return CurrentValues.Values.ToList().Sum(x => x.ContainersCount);
            }
        }

        public double ContainersPerHost
        {
            get
            {
                return CurrentValues.Values.ToList().Average(x => x.ContainersCount);
            }
        }

        public double AverageDataTotal
        {
            get
            {
                return CurrentValues.Values.Average(x => x.DataTotal);
            }
        }
        public double AverageDataIn
        {
            get
            {
                return CurrentValues.Values.Average(x => x.DataSizeIn);
            }
        }

        public double AverageDataOut
        {
            get
            {
                return CurrentValues.Values.Average(x => x.DataSizeOut);
            }
        }
    }
}
