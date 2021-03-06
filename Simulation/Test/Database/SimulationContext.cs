﻿using Simulation.Measure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Database
{
    public class SimulationContext:DbContext
    {
        public SimulationContext():base("SimulationContext")
        {

        }
        public DbSet<TrialResult> TrialResults { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrialResult>()
                .HasKey(t => new { t.Size,t.StartUtil,
                    t.Change,t.Algorithm,
                    t.TestedPercent, t.SchedulingAlgorithm,
                    t.TrialId,t.PushAuctionType,
                    t.PullAuctionType,t.ContainerType,
                    t.NetworkDelay});
            base.OnModelCreating(modelBuilder);
        }
    }
}
