using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Loads;

namespace Simulation.Helpers
{
    public class SimpleLinerRegression
    {
        private readonly List<Load> _loads;
        private Load _a;
        private Load _b;

        public SimpleLinerRegression(List<Load> loads )
        {
            _loads = Clone(loads);
            if (loads.Count < 3)
            {
                _a = new Load(0, 0, 0);
                _b = new Load(0,0,0);
                foreach (var load in loads)
                {
                    _b += load;
                }
                _b = _b/loads.Count;

            }
            else
            {
                ComputeSlope();
            }
        }

        private List<Load> Clone(List<Load> loads)
        {
            var r = new List<Load>();
            foreach (var load in loads)
            {
                r.Add(load);
            }
            return r;
        }

        private void ComputeSlope()
        {
            List<Load> values = Clone(_loads);
            double xAvg = 0;
            Load yAvg = new Load(0,0,0);

            for (int x = 0; x < values.Count; x++)
            {
                xAvg += x;
                yAvg += values[x];
            }

            xAvg = xAvg / values.Count;
            //if (xAvg == 0)
            //{
            //    xAvg++;
            //}
            yAvg = yAvg / values.Count;
            Load v1 = new Load(0,0,0);
            double v2 = 0;

            for (int x = 0; x < values.Count; x++)
            {
                v1 += (values[x] - yAvg) * (x - xAvg);
                v2 += Math.Pow(x - xAvg, 2);
            }
            //if (v2 == 0)
            //{
            //    v2 = 0.0000001;
            //}
            //else
            //{
                
            //}
            _a = v1 / v2;

            _b = yAvg - _a * xAvg;

            //Console.WriteLine("y = ax + b");
            //Console.WriteLine("a = {0}, the slope of the trend line.", _a);
            //Console.WriteLine("b = {0}, the intercept of the trend line.",_b);
        }

        public Load TimePredict(int t)
        {
            Load result = _a*t + _b;
            //Console.WriteLine($"result is {result}");
            if (result.CpuLoad < 0)
            {
                //throw new NotImplementedException("Simple linear reg");
               result = new Load(0.00001, 0.00001, 0.00001);
            }
            return result;
        }
    }
}
