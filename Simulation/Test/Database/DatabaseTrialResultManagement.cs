using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Measure;
using Simulation.MeasureResults;
using Test.Database;

namespace Test
{
    public class DatabaseTrialResultManagement : ITrialResultManagement
    {
        public void Save(MeasureValueHolder holder)
        {
            SimulationContext context = new SimulationContext();
            Func<TrialResult, bool> selecTionFunction = x => x.Size == ((int)holder.SimulationSize)
                                                             && x.StartUtil == holder.StartUtilization.ToString()
                                                             && x.Change == holder.ChangeAction.ToString()
                                                             && x.Algorithm == holder.Strategy.ToString()
                                                             && x.PushAuctionType == holder.Configuration.PushAuctionType.ToString()
                                                             && x.PullAuctionType == holder.Configuration.PullAuctionType.ToString()
                                                             && x.TestedPercent == (int)holder.TestedPercent
                                                             && x.SchedulingAlgorithm == holder.Scheduling.ToString()
                                                             && x.NetworkDelay == holder.NetworkDelay
                                                             && x.ContainerType == holder.ContainerType.ToString()
                                                             && x.TrialId == holder.TrialId;

            TrialResult t = context.TrialResults.SingleOrDefault(selecTionFunction)??new TrialResult();
            t.Size = ((int)holder.SimulationSize);
            t.StartUtil = holder.StartUtilization.ToString();
            t.Change = holder.ChangeAction.ToString();
            t.Algorithm = holder.Strategy.ToString();
            t.PushAuctionType = holder.Configuration.PushAuctionType.ToString();
            t.PullAuctionType = holder.Configuration.PullAuctionType.ToString();
            t.TestedPercent = (int)holder.TestedPercent;
            t.SchedulingAlgorithm = holder.Scheduling.ToString();
            t.ContainerType = holder.ContainerType.ToString();
            t.NetworkDelay = holder.NetworkDelay;
            t.TrialId = holder.TrialId;
            t.PredictionAlg = holder.Prediction.ToString();
            t.AverageEntropy = holder.AverageEntropy;
            t.Power = holder.PowerConsumption;
            t.StdDev = holder.AverageStdDeviation;
            t.Hosts = holder.AverageHosts;
            t.RMSE = holder.RMSE;
            t.TotalMessages = holder.TotalMessages;
            t.TotalCommunicatedData = holder.TotalCommunicatedSize;
            t.Migrations = holder.TotalMigrations;
            t.SlaViolations = holder.TotalSlaViolationsCount;
            t.SlaViolationsPercent = holder.AverageSlaViolationsPercent;
            t.ImagePullsTotal = holder.ImagePulls;
            t.ImagePullsRatio = holder.AveragePullPerImage;
            t.FinalEntropy = holder.FinalEntropy;
            t.ContainersAverage = holder.AverageContainers;
            t.AverageContainerPerHost = holder.AverageContainersPerHost;
            t.TotalContainers = holder.TotalContainers;
            t.AverageDownTime = holder.AverageDownTime;
            context.TrialResults.AddOrUpdate(t);
            context.SaveChanges();
        }
    }
}
