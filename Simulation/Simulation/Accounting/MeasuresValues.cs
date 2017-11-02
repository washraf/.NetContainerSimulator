using System.Collections.Generic;
using Simulation.Loads;

namespace Simulation.Accounting
{
    public class MeasuresValues
    {
        public MeasuresValues( double pushRequests, double pullRequests,
            double idealHostsCount, double noHosts,
            double migrations, double totalMessage,
            double entropy, double predictedEntropy,
            double pushLoadAvailabilityRequest, double pullLoadAvailabilityRequest,
            double avgNeededVol, double avgPredictedVol,double minNeeded,double maxNeeded,
            double underHosts, double overHosts,double slaViolations,double power,double stdDev)
        {
            PushRequests = pushRequests;
            PullRequests = pullRequests;
            IdealHostsCount = idealHostsCount;
            NoHosts = noHosts;
            Migrations = migrations;
            TotalMessages = totalMessage;
            Entropy = entropy;
            PredictedEntropy = predictedEntropy;
            PushLoadAvailabilityRequest = pushLoadAvailabilityRequest;
            PullLoadAvailabilityRequest = pullLoadAvailabilityRequest;
            AvgNeededVolume = avgNeededVol;
            AvgPredictedVolume = avgPredictedVol;
            MinNeeded = minNeeded;
            MaxNeeded = maxNeeded;
            UnderHosts = underHosts;
            OverHosts = overHosts;
            SlaViolations = slaViolations;
            Power = power;
            StdDev = stdDev;
        }
        public MeasuresValues(MeasuresValues listItem)
        {
            PushRequests = listItem.PushRequests;
            PullRequests = listItem.PullRequests;
            IdealHostsCount = listItem.IdealHostsCount;
            NoHosts = listItem.NoHosts;
            Migrations = listItem.Migrations;
            TotalMessages = listItem.TotalMessages;
            Entropy = listItem.Entropy;
            PredictedEntropy = listItem.PredictedEntropy;
            PushLoadAvailabilityRequest = listItem.PushLoadAvailabilityRequest;
            PullLoadAvailabilityRequest = listItem.PullLoadAvailabilityRequest;
            AvgPredictedVolume = listItem.AvgPredictedVolume;
            AvgNeededVolume = listItem.AvgNeededVolume;
            MinNeeded = listItem.MinNeeded;
            MaxNeeded = listItem.MaxNeeded;
            UnderHosts = listItem.UnderHosts;
            OverHosts = listItem.OverHosts;
            SlaViolations = listItem.SlaViolations;
            Power = listItem.Power;
            StdDev = listItem.StdDev;
        }

        public double PushRequests { get; private set; }
        public double PullRequests { get; private set; }
        public double IdealHostsCount { get; private set; }
        public double NoHosts { get; private set; }
        public double Migrations { get; private set; }
        public double TotalMessages { get; private set; }
        public double Entropy { get; private set; }
        public double PredictedEntropy { get; private set; }
        public double PushLoadAvailabilityRequest { get; private set; }
        public double PullLoadAvailabilityRequest { get; private set; }
        public double AvgNeededVolume { get; private set; }
        public double AvgPredictedVolume { get; private set; }
        public double MinNeeded { get; set; }
        public double MaxNeeded { get; set; }
        public double UnderHosts { get; private set; }
        public double OverHosts { get; private set; }
        public double SlaViolations { get; set; }
        public double Power { get; set; }
        public double StdDev { get; set; }


        public static MeasuresValues operator +(MeasuresValues first, MeasuresValues second)
        {
            return new MeasuresValues(first.PushRequests + second.PushRequests,
                first.PullRequests + second.PullRequests,
                first.IdealHostsCount + second.IdealHostsCount,
                first.NoHosts + second.NoHosts,
                first.Migrations + second.Migrations,
                first.TotalMessages + second.TotalMessages,
                first.Entropy + second.Entropy,
                first.PredictedEntropy + second.PredictedEntropy,
                first.PushLoadAvailabilityRequest + second.PushLoadAvailabilityRequest,
                first.PullLoadAvailabilityRequest + second.PullLoadAvailabilityRequest,
                first.AvgNeededVolume + second.AvgNeededVolume,
                first.AvgPredictedVolume + second.AvgPredictedVolume,
                first.MinNeeded + second.MinNeeded,
                first.MaxNeeded + second.MaxNeeded,
                first.UnderHosts + second.UnderHosts,
                first.OverHosts + second.OverHosts,
                first.SlaViolations+second.SlaViolations,
                first.Power+second.Power,
                first.StdDev+second.StdDev
                );
        }

        public static MeasuresValues operator /(MeasuresValues first, double count)
        {
            return new MeasuresValues(first.PushRequests / count,
                first.PullRequests / count,
                first.IdealHostsCount / count,
                first.NoHosts / count,
                first.Migrations / count,
                first.TotalMessages / count,
                first.Entropy / count,
                first.PredictedEntropy / count,
                first.PushLoadAvailabilityRequest / count,
                first.PullLoadAvailabilityRequest / count,
                first.AvgNeededVolume / count,
                first.AvgPredictedVolume / count,
                first.MinNeeded/count,
                first.MaxNeeded/count,
                first.UnderHosts / count,
                first.OverHosts / count,
                first.SlaViolations/count, 
                first.Power/count,
                first.StdDev/count);
        }


        

    }
}