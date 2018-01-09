using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Measure
{
    public partial class TrialResult
    {
        public int Size { get; set; }
        public string StartUtil { get; set; }
        public string Change { get; set; }
        public string Algorithm { get; set; }
        public double Entropy { get; set; }
        public double Power { get; set; }
        public double StdDev { get; set; }
        public double Hosts { get; set; }
        public double Migrations { get; set; }
        public double SlaViolations { get; set; }
        public double TotalMessages { get; set; }
        public string PredictionAlg { get; set; }
    }
}
