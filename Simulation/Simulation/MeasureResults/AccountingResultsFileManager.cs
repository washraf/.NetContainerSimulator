﻿using Simulation.Configuration;
using Simulation.Loads;
using Simulation.Measure;
using Simulation.SimulationController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.AccountingResults
{
    public class AccountingResultsFileManager:IAccountingResultsManager
    {

        public void WriteDataToDisk(MeasureValueHolder measureValueHolder)
        {
            string folder = @"D:\Simulations\Results\" +
                            (int)measureValueHolder.SimulationSize + "\\" +
                            measureValueHolder.StartUtilization + "_" + measureValueHolder.ChangeAction + "\\" +
                           measureValueHolder.Prediction + "\\" +
                           measureValueHolder.Scheduling + "\\" +
                            measureValueHolder.Strategy + "_" + measureValueHolder.ContainerType +"\\"+
                            measureValueHolder.Configuration.PushAuctionType + "_" + measureValueHolder.Configuration.PullAuctionType + "\\" +
                            measureValueHolder.TestedPercent + "\\"+measureValueHolder.NetworkDelay.ToString()+"\\" + 
                            measureValueHolder.TrialId + "\\";
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
                                     "NormalHosts," +
                                     "EvacuatingHosts," +
                                     "Migrations," +
                                     "PushRequests," +
                                     "PushLoadAvailabilityRequest," +
                                     "PullRequests," +
                                     "PullLoadAvailabilityRequest," +
                                     "TotalMessages," +
                                     "SlaViolationsCount," +
                                     "SlaViolationsPercent," +
                                     "MinNeeded," +
                                     "MaxNeeded," +
                                     "Power," +
                                     "stdDev,",
                                     "ImagePulls",
                                     "CommunicatedSize");

                    for (int i = 0; i < measureValueHolder.MeasuredValuesList.Count; i++)
                    {
                        var value = measureValueHolder.MeasuredValuesList[i];
                        writer.WriteLine($"{i}," +
                                         $"{value.Entropy}," +
                                         $"{value.PredictedEntropy}," +
                                         $"{value.AvgNeededVolume}," +
                                         $"{value.AvgPredictedVolume}," +
                                         $"{value.IdealHostsCount}," +
                                         $"{value.NoHosts}," +
                                         $"{value.UnderHosts}," +
                                         $"{value.OverHosts}," +
                                         $"{value.NormalHosts}," +
                                         $"{value.EvacuatingHosts}," +
                                         $"{value.Migrations}," +
                                         $"{value.PushRequests}," +
                                         $"{value.PushLoadAvailabilityRequest}," +
                                         $"{value.PullRequests}," +
                                         $"{value.PullLoadAvailabilityRequest}," +
                                         $"{value.TotalMessages}," +
                                         $"{value.SlaViolationsCount}," +
                                         $"{value.SlaViolationsPercentage}," +
                                         $"{value.MinNeeded}," +
                                         $"{value.MaxNeeded}," +
                                         $"{value.Power}," +
                                         $"{value.StdDev}," +
                                         $"{value.ImagePulls}," +
                                         $"{value.CommunicatedSize},");
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
                    foreach (var item in measureValueHolder.ContainerMeasureValuesList)
                    {
                        writer.WriteLine($"{item.Key}," +
                                         $"{item.Value.MigrationCount}," +
                                         $"{item.Value.Downtime}");
                    }
                }
                if (measureValueHolder.ContainerType == ContainersType.D)
                {
                    using (
                        StreamWriter writer =
                            new StreamWriter(folder + "PullsPerImage.txt", false))
                    {
                        writer.WriteLine("ImageId," +
                                         "Pulls");
                        foreach (var item in measureValueHolder.PullsPerImage)
                        {
                            writer.WriteLine($"{item.Key}," +
                                             $"{item.Value}");
                        }
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
                                     "ioutil,"+
                                     "DataSizeOut,"+
                                     "DataSizeIn"
                                     );
                    //public HostLoadInfo(int hostId, Load currentLoad, int containersCount, double cpu, double mem, double io)

                    for (int i = 0; i < measureValueHolder.HostMeasureValuesList.Count; i++)
                    {
                        foreach (
                            var item in measureValueHolder.HostMeasureValuesList[i].CurrentValues)
                        {
                            writer.WriteLine($"{i}," +
                                             $"{item.Value.ToString()}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Will create Directory");
                Directory.CreateDirectory(folder);
                this.WriteDataToDisk(measureValueHolder);
            }
        }

        public MeasureValueHolder ReadDataFromDisk(string mainFile)
        {
            var config = mainFile.Split('\\');
            SimulationSize simulationSize = (SimulationSize)Convert.ToInt32(config[3]);
            StartUtilizationPercent perecent =
               (StartUtilizationPercent)Enum.Parse(typeof(StartUtilizationPercent), config[4].Split('_')[0]);
            LoadChangeAction changeAction =
                (LoadChangeAction)Enum.Parse(typeof(LoadChangeAction), config[4].Split('_')[1]);

            LoadPrediction loadPrediction = (LoadPrediction)Enum.Parse(typeof(LoadPrediction), config[5]);
            SchedulingAlgorithm schedulingAlgorithm = (SchedulingAlgorithm)Enum.Parse(typeof(SchedulingAlgorithm), config[6]);
            Strategies strategy = (Strategies)Enum.Parse(typeof(Strategies), config[7].Split('_')[0]);
            ContainersType containerType = (ContainersType)Enum.Parse(typeof(ContainersType), config[7].Split('_')[1]);
            AuctionTypes pushAuctionType = (AuctionTypes)Enum.Parse(typeof(AuctionTypes), config[8].Split('_')[0]);
            AuctionTypes pullAuctionType = (AuctionTypes)Enum.Parse(typeof(AuctionTypes), config[8].Split('_')[1]);
            TestedHosts testedHosts = (TestedHosts)Enum.Parse(typeof(TestedHosts), config[9]);
            bool delay = bool.Parse(config[10]);

            int TrialId = int.Parse(config[11]);
            var conf = new RunConfiguration(simulationSize, perecent,changeAction,loadPrediction,strategy,pushAuctionType,pullAuctionType,schedulingAlgorithm, testedHosts,containerType,delay,TrialId);
            MeasureValueHolder holder =
                new MeasureValueHolder(conf);

            using (StreamReader reader = new StreamReader(mainFile))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(',');
                    int i = Convert.ToInt32(line[0]);
                    double entropy = Convert.ToDouble(line[1]);
                    double predictedEntropy = Convert.ToDouble(line[2]);
                    double avgRealVolume = Convert.ToDouble(line[3]);
                    double avgPredictedVolume = Convert.ToDouble(line[4]);
                    double idealHostCount = Convert.ToDouble(line[5]);
                    double noHosts = Convert.ToDouble(line[6]);
                    double underHosts = Convert.ToDouble(line[7]);
                    double overHosts = Convert.ToDouble(line[8]);
                    double normalHosts = Convert.ToDouble(line[9]);
                    double evacuatingHosts = Convert.ToDouble(line[10]);
                    double migrations = Convert.ToDouble(line[11]);
                    double pushRequests = Convert.ToDouble(line[12]);
                    double pushLoadAvailabilityRequest = Convert.ToDouble(line[13]);
                    double pullRequests = Convert.ToDouble(line[14]);
                    double pullLoadAvailabilityRequest = Convert.ToDouble(line[15]);
                    double totalMessages = Convert.ToDouble(line[16]);
                    double slaViolationsCount = Convert.ToDouble(line[17]);
                    double slaViolationsPercent = Convert.ToDouble(line[18]);

                    double minNeeded = Convert.ToDouble(line[19]);
                    double maxNeeded = Convert.ToDouble(line[20]);
                    double power = Convert.ToDouble(line[21]);
                    double stdDev = Convert.ToDouble(line[22]);
                    double imagePulls = Convert.ToDouble(line[23]);
                    double communicatedSize = Convert.ToDouble(line[24]);
                    MeasuresValues m = new MeasuresValues(pushRequests, pullRequests, idealHostCount, noHosts,
                        migrations,
                        totalMessages, entropy, predictedEntropy, pushLoadAvailabilityRequest,
                        pullLoadAvailabilityRequest,
                        avgRealVolume, avgPredictedVolume, minNeeded, maxNeeded,
                        underHosts, overHosts, normalHosts, evacuatingHosts,
                        slaViolationsCount,slaViolationsPercent,
                        power, stdDev, imagePulls,communicatedSize);
                    holder.MeasuredValuesList.Add(m);
                }
            }

            var nfile = mainFile.Replace("All", "ConMig");
            using (StreamReader reader = new StreamReader(new FileStream(nfile, FileMode.Open)))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(',');
                    int conId = Convert.ToInt32(line[0]);
                    double count = Convert.ToDouble(line[1]);
                    double time = Convert.ToDouble(line[2]);
                    holder.ContainerMeasureValuesList.Add(conId, new ContainerMeasureValue(conId, count, time));
                }
            }

            nfile = mainFile.Replace("All", "Hosts");
            using (StreamReader reader = new StreamReader(new FileStream(nfile, FileMode.Open)))
            {
                List<HostLoadInfo> list = new List<HostLoadInfo>();
                int current = 0;
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    //iteration,Id,cpu,mem,io,concount,cpuutil,memutil,ioutil,
                    var line = reader.ReadLine().Split(',');
                    int it = Convert.ToInt32(line[0]);
                    int hostId = Convert.ToInt32(line[1]);
                    double cpu = Convert.ToDouble(line[2]);
                    double mem = Convert.ToDouble(line[3]);
                    double io = Convert.ToDouble(line[4]);
                    int concount = Convert.ToInt32(line[5]);
                    double cpuutil = Convert.ToDouble(line[6]);
                    double memutil = Convert.ToDouble(line[7]);
                    double ioutil = Convert.ToDouble(line[8]);
                    double dataSizeOut = Convert.ToDouble(line[9]);
                    double dataSizeIn = Convert.ToDouble(line[10]);

                    var linfo = new HostLoadInfo(hostId, new Load(cpu, mem, io), concount, cpuutil, memutil, ioutil,dataSizeOut,dataSizeIn);
                    if (it == current)
                    {
                        list.Add(linfo);
                    }
                    else
                    {
                        holder.HostMeasureValuesList.Add(new HostMeasureValues(list));
                        list.Clear();
                        list.Add(linfo);
                        current++;
                    }
                }
            }
            if (containerType == ContainersType.D)
            {
                nfile = mainFile.Replace("All", "PullsPerImage");
                using (StreamReader reader = new StreamReader(new FileStream(nfile, FileMode.Open)))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine().Split(',');
                        int ImageId = Convert.ToInt32(line[0]);
                        int Pulls = Convert.ToInt32(line[1]);
                        holder.PullsPerImage.Add(ImageId, Pulls);
                    }
                }
            }

            return holder;
        }
        
    }
}
