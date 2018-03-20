using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Measure
{
    public class DBEHolder
    {
        public double Power { set; get; }
        public double Hosts { set; get; }
        public double RMSE { set; get; }
        public double Migrations { set; get; }
        public double Messages { set; get; }
        public double Entropy { set; get; }
        public double SLA { set; get; }

        public static double GetDBE (DBEHolder target, DBEHolder max)
        {
            var normal = GetNormalizedDBE(target, max);

            var properties = normal.GetType().GetProperties();
            double result = 0;
            foreach (var property in properties)
            {
                var n = double.Parse(property.GetValue(normal).ToString());
                if (property.Name == "Entropy")
                    n = (1 - n);
                if(!double.IsNaN(n))
                    result += Math.Pow(1-n,2);
            }
            result = Math.Sqrt(result);
            return result;
        }

        private static DBEHolder GetNormalizedDBE (DBEHolder target, DBEHolder max)
        {
            var normal = new DBEHolder
            {
                Entropy = target.Entropy / max.Entropy,
                Hosts = target.Hosts / max.Hosts,
                RMSE = target.RMSE / max.RMSE,
                SLA = target.SLA / max.SLA,
                Messages = target.Messages / max.Messages,
                Migrations = target.Migrations / max.Migrations,
                Power = target.Power / max.Power,
            };
            return normal;
        }
    }
}
