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
                                                             && x.PullAcutionType == holder.Configuration.PullAuctionType.ToString()
                                                             && x.Tested == holder.Tested.ToString()
                                                             && x.SchedulingAlgorithm == holder.Scheduling.ToString()
                                                             && x.TrialId == holder.TrialId;
            TrialResult t = context.TrialResults.SingleOrDefault(selecTionFunction)??new TrialResult();
            t.Size = ((int)holder.SimulationSize);
            t.StartUtil = holder.StartUtilization.ToString();
            t.Change = holder.ChangeAction.ToString();
            t.Algorithm = holder.Strategy.ToString();
            t.PushAuctionType = holder.Configuration.PushAuctionType.ToString();
            t.PullAcutionType = holder.Configuration.PullAuctionType.ToString();
            t.Tested = holder.Tested.ToString();
            t.SchedulingAlgorithm = holder.Scheduling.ToString();
            t.TrialId = holder.TrialId;
            t.PredictionAlg = holder.Prediction.ToString();
            t.AverageEntropy = holder.AverageEntropy;
            t.Power = holder.PowerConsumption;
            t.StdDev = holder.AverageStdDeviation;
            t.Hosts = holder.AverageHosts;
            t.RMSE = holder.RMSE;
            t.TotalMessages = holder.TotalMessages;
            t.Migrations = holder.TotalMigrations;
            t.SlaViolations = holder.TotalSlaViolations;
            t.ImagePullsTotal = holder.ImagePulls;
            t.ImagePullsRatio = holder.AveragePullPerImage;
            t.FinalEntropy = holder.FinalEntropy;
            t.ContainersAverage = holder.AverageContainers;
            t.TotalContainers = holder.TotalContainers;
            context.TrialResults.AddOrUpdate(t);
            context.SaveChanges();
        }
    }
}
