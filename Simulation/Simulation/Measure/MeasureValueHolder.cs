using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Simulation.Configuration;
using Simulation.LocationStrategies;
using Simulation.SimulationController;

namespace Simulation.Measure
{
    public class MeasureValueHolder
    {
       // private MeasureValueHolder measureValueHolder;

        public Strategies Strategy { get; set; }
        public SimulationSize SimulationSize { get; set; }
        public StartUtilizationPercent StartUtilization { get; set; }
        public LoadChangeAction ChangeAction { get; set; }
        public LoadPrediction Prediction { get; set; }
        public SchedulingAlgorithm Scheduling { get; }
        public int TrialId { get; }
        public TestedHosts TestedPercent { get; }
        public ContainersType ContainerType { get; }

        public string Name
        {
            get
            {
                return (int)SimulationSize + "_" + (int)StartUtilization
                    + "_" + ChangeAction.ToString()
                    + "_" + Prediction.ToString() + "_" + Scheduling.ToString()
                    + "_" + Strategy.ToString() + "_" + ContainerType
                    + "_" + Configuration.PushAuctionType.ToString() + "_" + Configuration.PullAuctionType
                    + "_" + TestedPercent.ToString() + "_" + TrialId;
            }
        }

        public MeasureValueHolder(RunConfiguration configuration)
        {
            Strategy = configuration.Strategy;
            SimulationSize = configuration.SimulationSize;
            StartUtilization = configuration.StartPercent;
            ChangeAction = configuration.ChangeAction;
            Prediction = configuration.LoadPrediction;
            TestedPercent = configuration.TestedHosts;
            Scheduling = configuration.SchedulingAlgorithm;
            ContainerType = configuration.ContainersType;
            Configuration = configuration;
            TrialId = configuration.TrialId;
        }

        

        public List<MeasuresValues> MeasuredValuesList { set; get; } = new List<MeasuresValues>();
        public List<HostMeasureValues> HostMeasureValuesList { set; get; } = new List<HostMeasureValues>(); 

        public Dictionary<int, ContainerMeasureValue> ContainerMigrationCount { get; set; } =
            new Dictionary<int, ContainerMeasureValue>();
        public Dictionary<int, int> PullsPerImage { get; set; } = 
            new Dictionary<int, int>();

        #region --Operations --
        public static MeasureValueHolder operator +(MeasureValueHolder first, MeasureValueHolder second)
        {
            if (first.Name != second.Name)
            {
                throw new ArgumentOutOfRangeException();
            }
            MeasureValueHolder final = new MeasureValueHolder(first.Configuration);
            //Prepare for first
            foreach (var listItem in first.MeasuredValuesList)
            {
                final.MeasuredValuesList.Add(new MeasuresValues(listItem));
            }
            //Hosts
            foreach (var item in first.HostMeasureValuesList)
            {
                final.HostMeasureValuesList.Add(new HostMeasureValues(item));
            }

            //Container Migrations
            foreach (var item in first.ContainerMigrationCount)
            {
                final.ContainerMigrationCount.Add(item.Key, item.Value);
            }
            //Images
            foreach (var item in first.PullsPerImage)
            {
                final.PullsPerImage.Add(item.Key, item.Value);
            }





            //prepare of second
            for (int i = 0; i < second.MeasuredValuesList.Count; i++)
            {
                if (i < final.MeasuredValuesList.Count)
                    final.MeasuredValuesList[i] += second.MeasuredValuesList[i];
                else
                {
                    final.MeasuredValuesList.Add(new MeasuresValues(second.MeasuredValuesList[i]));
                }
            }
            //hosts
            for (int i = 0; i < second.HostMeasureValuesList.Count; i++)
            {
                if (i < final.HostMeasureValuesList.Count)
                    final.HostMeasureValuesList[i] += second.HostMeasureValuesList[i];
                else
                {
                    final.HostMeasureValuesList.Add(new HostMeasureValues(second.HostMeasureValuesList[i]));
                }
            }
            //contianers
            foreach (var item in second.ContainerMigrationCount)
            {
                if (final.ContainerMigrationCount.ContainsKey(item.Key))
                    final.ContainerMigrationCount[item.Key] += item.Value;
                else
                {
                    final.ContainerMigrationCount.Add(item.Key, item.Value);
                }
            }

            //Images
            foreach (var item in second.PullsPerImage)
            {
                if (final.PullsPerImage.ContainsKey(item.Key))
                    final.PullsPerImage[item.Key] += item.Value;
                else
                    final.PullsPerImage.Add(item.Key, item.Value);
            }


            return final;
        }
        public static MeasureValueHolder operator /(MeasureValueHolder first, int c)
        {
            MeasureValueHolder final = new MeasureValueHolder(first.Configuration);
            foreach (var listItem in first.MeasuredValuesList)
            {
                final.MeasuredValuesList.Add(listItem/c);
            }

            foreach (var item in first.ContainerMigrationCount)
            {
                final.ContainerMigrationCount.Add(item.Key, item.Value/c);
            }
            foreach (var item in first.HostMeasureValuesList)
            {
                final.HostMeasureValuesList.Add(item/c);
            }
            foreach (var item in first.PullsPerImage)
            {
                final.PullsPerImage.Add(item.Key, item.Value / c);
            }

            return final;
        }
        #endregion

        #region --aggregates --
        public double AverageHosts
        {
            get { return MeasuredValuesList.Select(x => x.NoHosts).Average(); }
        }

        public double RMSE
        {
            get {
                var rmse = 0.0;
                var l1 = MeasuredValuesList.Select(x => x.IdealHostsCount).ToList();
                var l2 = MeasuredValuesList.Select(x => x.NoHosts).ToList();
                for (int i = 0; i < l1.Count(); i++)
                {
                    rmse += Math.Pow(l1[i] - l2[i],2);
                }
                rmse  /= l1.Count();
                rmse = Math.Sqrt(rmse);
                return rmse;
            }
        }

        public double AveragUtilization
        {
            get { return MeasuredValuesList.Select(x => x.AvgNeededVolume).Average(); }
        }
        public double TotalMessages
        {
            get { return MeasuredValuesList.Select(x => x.TotalMessages).Sum(); }
        }

        public double AverageEntropy
        {
            get { return MeasuredValuesList.Select(x => x.Entropy).Average(); }
        }
        public double FinalEntropy
        {
            get { return MeasuredValuesList.Select(x => x.Entropy).Last(); }
        }


        public double FinalUnderUtilized
        {
            get { return MeasuredValuesList.Select(x => x.UnderHosts).Last(); }
        }

        public double FinalOverUtilized
        {
            get { return MeasuredValuesList.Select(x => x.OverHosts).Last(); }
        }
        public double FinalNormalUtilized
        {
            get { return MeasuredValuesList.Select(x => x.NormalHosts).Last(); }
        }

        public double FinalEvacuatingUtilized
        {
            get { return MeasuredValuesList.Select(x => x.EvacuatingHosts).Last(); }
        }

        public double TotalPushRequests
        {
            get { return MeasuredValuesList.Select(x => x.PushRequests).Sum(); }
        }

        public double TotalPullRequests
        {
            get { return MeasuredValuesList.Select(x => x.PullRequests).Sum(); }
        }

        public double TotalMigrations
        {
            get { return MeasuredValuesList.Select(x => x.Migrations).Sum(); }
        }

        public double AvgMigrations
        {
            get { return ContainerMigrationCount.Select(x => x.Value.MigrationCount).Average(); }
        }

        public double TotalPushAvailabilityRequests
        {
            get { return MeasuredValuesList.Select(x => x.PushLoadAvailabilityRequest).Sum(); }
        }

        public double TotalPullAvailabilityRequests
        {
            get { return MeasuredValuesList.Select(x => x.PullLoadAvailabilityRequest).Sum(); }
        }

        public double TotalSlaViolations
        {
            get { return MeasuredValuesList.Select(x => x.SlaViolations).Sum(); }
        }

        public double AvgDownTime
        {
            get { return ContainerMigrationCount.Values.Select(x => x.Downtime*1.0).Average(); }
        }

        public double PowerConsumption
        {
            get { return MeasuredValuesList.Sum(x => x.Power); }
        }

        public double AverageStdDeviation
        {
            get { return MeasuredValuesList.Average(x => x.StdDev); }
        }

        public double ImagePulls
        {
            get { return MeasuredValuesList.Sum(x => x.ImagePulls); }
        }

        public double AveragePullPerImage
        {
            get {
                return Configuration.ContainersType == ContainersType.D?
                     PullsPerImage.Select(x => x.Value).Average():0; }
        }

        public double AverageContainers
        {
            get
            {
                return HostMeasureValuesList.Average(x => x.Containers);
            }
        }
        public double AverageContainersPerHost {
            get
            {
                return HostMeasureValuesList.Average(x => x.ContainersPerHost);
            }
        }

        public RunConfiguration Configuration { get; }
        public double TotalContainers {

            get
            {
                return HostMeasureValuesList.Last().Containers;
            }

        }

        public double TotalCommunicatedSize
        {
            get {
                return MeasuredValuesList.Sum(x => x.CommunicatedSize);
            }
        }

        public double AverageDataOut
        {
            get
            {
                return HostMeasureValuesList.Average(x => x.AverageDataOut);
            }
        }
        public double AverageDataIn
        {
            get
            {

                return HostMeasureValuesList.Average(x => x.AverageDataIn);
            }
        }

        public double AverageDataTotal
        {
            get
            {
                return HostMeasureValuesList.Average(x => x.AverageDataTotal);
            }
        }


        #endregion

    }
}