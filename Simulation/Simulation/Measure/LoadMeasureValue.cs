using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.Measure
{
    public class LoadMeasureValue
    {
        private LoadMeasureValue()
        {
            
        }
        public LoadMeasureValue(List<HostLoadInfo> loads)
        {
            foreach (var l in loads)
            {
                CurrentValues.Add(l.HostId,l);
            }
        }

        public LoadMeasureValue(LoadMeasureValue item)
        {
            foreach (var l in item.CurrentValues.Values)
            {
                CurrentValues.Add(l.HostId,l);
            }
        }

        /// <summary>
        /// Key => Host Id
        /// Value => Host Load Info for this id
        /// </summary>
        public Dictionary<int,HostLoadInfo> CurrentValues { set; get; } = new Dictionary<int, HostLoadInfo>();

        public static LoadMeasureValue operator +(LoadMeasureValue first, LoadMeasureValue second)
        {
            var final = new LoadMeasureValue();
            foreach (var l in first.CurrentValues.Values)
            {
                final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId, l.CurrentLoad,l.ContainersCount,l.CPUUtil,l.MemoryUtil,l.IOUtil));
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
                        final.CurrentValues[l.HostId].IOUtil + l.IOUtil
                        );
                }
                else
                {
                    final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId, l.CurrentLoad,l.ContainersCount, l.CPUUtil, l.MemoryUtil, l.IOUtil));
                }
            }
            return final;
        }

        public static LoadMeasureValue operator /(LoadMeasureValue first, int c)
        {
            var final = new LoadMeasureValue();
            foreach (var l in first.CurrentValues.Values)
            {
                final.CurrentValues.Add(l.HostId, new HostLoadInfo(l.HostId,l.CurrentLoad/c,l.ContainersCount/c,l.CPUUtil/c,l.MemoryUtil/c,l.IOUtil/c));
            }
            return final;
        }


        //Aggregates
        public double StandardDeviation
        {
            get { return CurrentValues.Values.ToList().StandardDeviation(); }
        }
    }
}
