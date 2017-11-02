using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.Loads;
using Simulation.LocationStrategies;

namespace Simulation.Helpers
{
    public static class AccountingHelpers
    {
        public static double CalculateEntropy(List<HostLoadInfo> loads)
        {
            List<double> a = new List<double>();
            foreach (var l in loads)
            {
                a.Add(l.Volume);
            }
            List<double> p = new List<double>();
            var aSum = a.Sum();
            foreach (var ai in a)
            {
                p.Add(ai / aSum);
            }
            var res = p.Select(pi => pi * Math.Log(pi, 2));
            var ressum = res.Sum();
            var final = ressum / Math.Log(loads.Count, 2) * -1;
            return final;
        }

        public static double StandardDeviation(this List<HostLoadInfo> values)
        {
            double avg = values.Select(x => x.Volume).Average();
            return Math.Sqrt(values.Select(x=>x.Volume).Average(v => Math.Pow(v - avg, 2)));
        }

        public static double PowerConsumption(this List<HostLoadInfo> values)
        {
            double consumption = 0;
            Dictionary<int, float> util = new Dictionary<int, float>();
            util.Add(0, 124);
            util.Add(25, 168);
            util.Add(50, 191);
            util.Add(75, 217);
            util.Add(100, 239);
            double t = 1.0*Global.CheckRate/Global.Second;
            t /= 3600;

            foreach (var val in values)
            {
                var avgutil = val.Volume*100;
                avgutil = avgutil > 100 ? 100 : avgutil;
                int min, max = 100;
                min = 0;
                foreach (var u in util)
                {
                    if (u.Key <= avgutil)
                    {
                        min = u.Key;
                    }
                    else
                    {
                        max = u.Key;
                        break;
                    }
                }
                var step = (util[max] - util[min])/25;
                var consPerUtil = (step*(avgutil - min)) + util[min];
                consumption += (t*consPerUtil);
            }
            double extra = 0;
            var hosts = values.Count;
            if (Global.CurrentStrategy == Strategies.InOrderProping)
            {
                var e = hosts*1.05 < (int) Global.SimulationSize ? hosts*0.05 : ((int) Global.SimulationSize - hosts);
                extra = e*t*util[0];
            }

            consumption += +extra;

            return consumption;
        }
    }
}
