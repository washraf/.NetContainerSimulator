using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Simulation.Helpers
{
    public static class RandomNumberGenerator
    {
        static RandomNumberGenerator()
        {
        }

        static int nextHost = 1;
        static int nextContainer = 1;
        static int nextInstance = 1;
        static object lck = new object();
        public static int GetHostRandomNumber()
        {
            lock (lck)
            {
                return nextHost++;
            }
        }

        public static int GetContainerRandomNumber()
        {
            lock (lck)
            {
                return nextContainer++;
            }
        }

        public static int GetInstanceRandomNumber()
        {
            lock (lck)
            {
                return nextInstance++;
            }
        }

        public static void ClearRandomNumber()
        {
            lock (lck)
            {
                nextHost = 1;
                nextContainer = 1;
                nextInstance = 1;
            }
        }
        /// <summary>
        ///   Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
        /// </summary>
        /// <param name="r"></param>
        /// <param name = "mean">Mean of the distribution</param>
        /// <param name = "stdDiv">Standard deviation</param>
        /// <returns></returns>
        public static double NextGaussian(this Random r, double mean = 0, double stdDiv = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = mean + stdDiv * rand_std_normal;

            return rand_normal;
        }


        public static int GetRandomFromDictionary(this Random r, Dictionary<int, double> list  )
        {
            double p = r.NextDouble();
            double cumulativeProbability = 0.0;
            foreach (var item in list)
            {
                cumulativeProbability += item.Value;
                if (p <= cumulativeProbability)
                {
                    return item.Key;
                }
            }
            throw new ArgumentException("How coma all less than value");
        }

        public static ContainerToHost GetRandomFromContainerToHost(this Random r, List<ContainerToHost> list )
        {
            double p = r.NextDouble();
            double cumulativeProbability = 0.0;
            foreach (var item in list)
            {
                cumulativeProbability += item.Probaility;
                if (p <= cumulativeProbability)
                {
                    return item;
                }
            }
            throw new ArgumentException("How coma all less than value");
        }
    }
}
