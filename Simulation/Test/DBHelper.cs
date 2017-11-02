using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Accounting;

namespace Test
{
    public class DbHelper
    {
        public static void SaveToDb(MeasureValueHolder holder)
        {
            SimulationEntities context = new SimulationEntities();
            Trial t = context.Trials.FirstOrDefault(x => x.Size == ((int)holder.SimulationSize)
                                                         && x.StartUtil == holder.StartUtilization.ToString()
                                                         && x.Change == holder.ChangeAction.ToString()
                                                         && x.Algorithm == holder.Strategy.ToString()) ?? new Trial();
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
            context.Trials.AddOrUpdate(t);
            context.SaveChanges();
        }
    }
}
