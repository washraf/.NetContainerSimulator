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
                                                             && x.Algorithm == holder.Strategy.ToString();
            TrialResult t = context.TrialResults.FirstOrDefault(selecTionFunction)??new TrialResult();
            t.Size = ((int)holder.SimulationSize);
            t.StartUtil = holder.StartUtilization.ToString();
            t.Change = holder.ChangeAction.ToString();
            t.Algorithm = holder.Strategy.ToString();
            t.PredictionAlg = holder.Prediction.ToString();
            t.Entropy = holder.AverageEntropy;
            t.Power = holder.PowerConsumption;
            t.StdDev = holder.AverageStdDeviation;
            t.Hosts = holder.AverageHosts;
            t.TotalMessages = holder.TotalMessages;
            t.Migrations = holder.TotalMigrations;
            t.SlaViolations = holder.TotalSlaViolations;
            context.TrialResults.AddOrUpdate(t);
            context.SaveChanges();
        }
    }
}
