using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Simulation.Configuration;
using Simulation.LocationStrategies;

namespace Simulation.Accounting
{
    public class MeasureValueHolder
    {
        private MeasureValueHolder measureValueHolder;

        public Strategies Strategy { get; set; }
        public SimulationSize SimulationSize { get; set; }
        public StartUtilizationPercent StartUtilization { get; set; }
        public LoadChangeAction ChangeAction { get; set; }
        public LoadPrediction Prediction { get; set; }
        public TestedHosts Tested { get; }

        public string Name
        {
            get
            {
                return SimulationSize.ToString() + "_" + StartUtilization.ToString() + "_" + ChangeAction.ToString() +
                       "_" + Prediction.ToString() + "_" + Strategy.ToString()+"_"+Tested;
            }
        }

        public MeasureValueHolder(Strategies strategy, SimulationSize simulationSize,
            StartUtilizationPercent startUtilization, LoadChangeAction change, LoadPrediction prediction,
            TestedHosts tested)
        {
            Strategy = strategy;
            SimulationSize = simulationSize;
            StartUtilization = startUtilization;
            ChangeAction = change;
            Prediction = prediction;
            Tested = tested;
        }

        

        public List<MeasuresValues> MeasuredValuesList { set; get; } = new List<MeasuresValues>();
        public List<LoadMeasureValue> LoadMeasureValueList { set; get; } = new List<LoadMeasureValue>(); 

        public Dictionary<int, ContainerMeasureValue> ContainerMigrationCount { get; set; } =
            new Dictionary<int, ContainerMeasureValue>();
        #region --Operations --
        public static MeasureValueHolder operator +(MeasureValueHolder first, MeasureValueHolder second)
        {
            if (first.Name != second.Name)
            {
                throw new ArgumentOutOfRangeException();
            }
            MeasureValueHolder final = new MeasureValueHolder(first.Strategy, first.SimulationSize,
                first.StartUtilization, first.ChangeAction, first.Prediction,first.Tested);
            //Prepare for first
            foreach (var listItem in first.MeasuredValuesList)
            {
                final.MeasuredValuesList.Add(new MeasuresValues(listItem));
            }
            foreach (var item in first.ContainerMigrationCount)
            {
                final.ContainerMigrationCount.Add(item.Key, item.Value);
            }

            foreach (var item in first.LoadMeasureValueList)
            {
                final.LoadMeasureValueList.Add(new LoadMeasureValue(item));
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

            foreach (var item in second.ContainerMigrationCount)
            {
                if (final.ContainerMigrationCount.ContainsKey(item.Key))
                    final.ContainerMigrationCount[item.Key] += item.Value;
                else
                {
                    final.ContainerMigrationCount.Add(item.Key, item.Value);
                }
            }

            for (int i = 0; i < second.LoadMeasureValueList.Count; i++)
            {
                if (i < final.LoadMeasureValueList.Count)
                    final.LoadMeasureValueList[i] += second.LoadMeasureValueList[i];
                else
                {
                    final.LoadMeasureValueList.Add(new LoadMeasureValue(second.LoadMeasureValueList[i]));
                }
            }

            return final;
        }
        public static MeasureValueHolder operator /(MeasureValueHolder first, int c)
        {
            MeasureValueHolder final = new MeasureValueHolder(first.Strategy, first.SimulationSize,
                first.StartUtilization, first.ChangeAction, first.Prediction,first.Tested);
            foreach (var listItem in first.MeasuredValuesList)
            {
                final.MeasuredValuesList.Add(listItem/c);
            }

            foreach (var item in first.ContainerMigrationCount)
            {
                final.ContainerMigrationCount.Add(item.Key, item.Value/c);
            }
            foreach (var item in first.LoadMeasureValueList)
            {
                final.LoadMeasureValueList.Add(item/c);
            }
            return final;
        }
        #endregion

        #region --aggregates --
        public double AverageHosts
        {
            get { return MeasuredValuesList.Select(x => x.NoHosts).Average(); }
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

        public double TotalUnderUtilized
        {
            get { return MeasuredValuesList.Select(x => x.UnderHosts).Sum(); }
        }

        public double TotalOverUtilized
        {
            get { return MeasuredValuesList.Select(x => x.OverHosts).Sum(); }
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
        #endregion

        #region --Write To Disk--
        public void WriteDataToDisk(int trialno)
        {
            string folder = @"D:\Simulations\Results\" +
                            (int) this.SimulationSize + "\\" +
                            this.StartUtilization + "_" + ChangeAction + "\\" +
                            Prediction + "\\" + DateTime.Now.ToShortDateString() + "\\" +
                            Strategy + "\\"+Tested+"\\";
                    if(trialno!=-1)
                           folder+=trialno + "\\";
            try
            {
                using (
                    StreamWriter writer =
                        new StreamWriter(folder + "All.txt", false))
                {
                    writer.WriteLine("i," +
                                     "Entropy," +
                                     "PrediectedEntropy," +
                                     "AvgNeededVolume," +
                                     "AvgPredictedVolume," +
                                     "IdealHostsCount," +
                                     "NoHosts," +
                                     "UnderHosts," +
                                     "OverHosts," +
                                     "Migrations," +
                                     "PushRequests," +
                                     "PushLoadAvailabilityRequest," +
                                     "PullRequests," +
                                     "PullLoadAvailabilityRequest," +
                                     "TotalMessages," +
                                     "SlaViolations," +
                                     "MinNeeded," +
                                     "MaxNeeded," +
                                     "Power," +
                                     "stdDev");

                    for (int i = 0; i < this.MeasuredValuesList.Count; i++)
                    {
                        var value = this.MeasuredValuesList[i];
                        writer.WriteLine($"{i}," +
                                         $"{value.Entropy}," +
                                         $"{value.PredictedEntropy}," +
                                         $"{value.AvgNeededVolume}," +
                                         $"{value.AvgPredictedVolume}," +
                                         $"{value.IdealHostsCount}," +
                                         $"{value.NoHosts}," +
                                         $"{value.UnderHosts}," +
                                         $"{value.OverHosts}," +
                                         $"{value.Migrations}," +
                                         $"{value.PushRequests}," +
                                         $"{value.PushLoadAvailabilityRequest}," +
                                         $"{value.PullRequests}," +
                                         $"{value.PullLoadAvailabilityRequest}," +
                                         $"{value.TotalMessages}," +
                                         $"{value.SlaViolations}," +
                                         $"{value.MinNeeded}," +
                                         $"{value.MaxNeeded}," +
                                         $"{value.Power}," +
                                         $"{value.StdDev}");

                    }
                    writer.Flush();
                }

                using (
                    StreamWriter writer =
                        new StreamWriter(folder + "ConMig.txt", false))
                {
                    writer.WriteLine("id," +
                                     "Migrations" +
                                     "DTime");
                    foreach (var item in this.ContainerMigrationCount)
                    {
                        writer.WriteLine($"{item.Key}," +
                                         $"{item.Value.MigrationCount}," +
                                         $"{item.Value.Downtime}");
                    }
                }

                using (
                    StreamWriter writer =
                        new StreamWriter(folder + "Hosts.txt", false))
                {
                    writer.WriteLine("iteration," +
                                     "Id," +
                                     "cpu," +
                                     "mem," +
                                     "io," +
                                     "concount," +
                                     "cpuutil," +
                                     "memutil," +
                                     "ioutil,"
                                     );
                    //public HostLoadInfo(int hostId, Load currentLoad, int containersCount, double cpu, double mem, double io)

                    for (int i = 0; i < this.LoadMeasureValueList.Count; i++)
                    {
                        foreach (
                            var item in this.LoadMeasureValueList[i].CurrentValues)
                        {
                            writer.WriteLine($"{i}," +
                                             $"{item.Value}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Will create Directory");
                Directory.CreateDirectory(folder);
                this.WriteDataToDisk(trialno);
            }
        }
        #endregion
    }
}