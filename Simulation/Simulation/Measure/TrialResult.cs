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
        public string SchedulingAlgorithm { get; set; }
        public string Algorithm { get; set; }
        public string PushAuctionType { get; set; }
        public string PullAuctionType { get; set; }
        public int TestedPercent { get; set; }
        public int TrialId { get; set; }
        //Results
        public double AverageEntropy { get; set; }
        public double FinalEntropy { get; set; }

        public double Power { get; set; }
        public double StdDev { get; set; }
        public double Hosts { get; set; }
        public double Migrations { get; set; }
        public double AverageDownTime { get; set; }
        public double SlaViolations { get; set; }
        public double SlaViolationsPercent { get; set; }

        public double TotalMessages { get; set; }
        public string PredictionAlg { get; set; }
        public double ImagePullsTotal { get; set; }
        public double ImagePullsRatio { get; set; }
        public double ContainersAverage { get; set; }
        public double TotalContainers { get; set; }
        public double AverageContainerPerHost { get; set; }

        public double RMSE { get; set; }
    }
}
