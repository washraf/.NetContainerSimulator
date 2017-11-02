using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.Helpers;

namespace Simulation.Loads
{

    public class LoadGenerator
    {
        
        //public static Load GetStartLoad(int conId)
        //{
        //    //switch (Global.LoadCreationStrategy)
        //    //{
        //    //    case LoadCreationStrategy.Fixed:
        //    //        return GetFixedLoad();
        //    //    case LoadCreationStrategy.Selected:
        //    //        return GetSelectedLoad(conId);
        //    //    case LoadCreationStrategy.Random:
        //    //        return GetRandomLoadInRange();
        //    //    default:
        //    //        throw new ArgumentOutOfRangeException();
        //    //}
            
        //}

        public static Load GetUpdatedContainerLoad(int conId,LoadChangeAction changeMethod,Load old)
        {
            var x = 1.0+((int) changeMethod/100.0);
            return old * x;
        }

        //private static Load GetFixedLoad()
        //{
        //    return Global.ContainerLoadNormal;
        //}
        //private static Load GetSelectedLoad(int conId)
        //{
        //    var l = GetLoadWithRespectToId(conId);
        //    return l;
        //}
        //private static Load GetRandomLoadInRange()
        //{
        //    Load start = Global.ContainerLoadPreNormal, end = Global.ContainerLoadPostNormal;
        //    double cpu, mem, io;
        //    Random r = new Random();
        //    cpu = r.Next((int)start.CpuLoad, (int)end.CpuLoad);
        //    mem = r.Next((int)start.MemorySize, (int)end.MemorySize);
        //    io = r.Next((int)start.IoSecond, (int)end.IoSecond);
        //    return new Load(cpu, mem, io);
        //}

        private static Load GetRandomLoad()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            var r = random.NextGaussian(4,1);
            var m = random.Next(0,4);
            var load = new Load(r, m* 256 + 256, r);
            return load;
        }

        public static List<Load> GenerateLoadList(StartUtilizationPercent utilizationPercent, SimulationSize currentSimulationSize)
        {
            List<Load> loads = new List<Load>();
            int max = (int)utilizationPercent;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            max = max + random.Next(-20, 20);
            //max *= (int) currentSimulationSize;
            int c = 0;
            double total = 0;
            while (total < max)
            {

                var l = GetRandomLoad();
                loads.Add(l);
                total += l.CpuLoad;
                c++;
            }
            //conList.RemoveAt(conList.Count-1);
            var cpu = loads.Select(x => x.CpuLoad).Sum();
            var mem = loads.Select(x => x.MemorySize).Sum();
            return loads;
        }
    }
}
