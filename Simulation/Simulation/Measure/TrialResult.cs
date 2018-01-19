using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Measure
{
    public partial class TrialResult
    {
        //Keys
        public int Size { get; set; }
        public string StartUtil { get; set; }
        public string Change { get; set; }
        public string Algorithm { get; set; }
        public int Tested { get; set; }
        //Results
        public double AverageEntropy { get; set; }
        public double FinalEntropy { get; set; }

        public double Power { get; set; }
        public double StdDev { get; set; }
        public double Hosts { get; set; }
        public double Migrations { get; set; }
        public double SlaViolations { get; set; }
        public double TotalMessages { get; set; }
        public string PredictionAlg { get; set; }
        public double ImagePullsTotal { get; set; }
        public double ImagePullsRatio { get; set; }
        public double ContainersAverage { get; set; }
    }
}
