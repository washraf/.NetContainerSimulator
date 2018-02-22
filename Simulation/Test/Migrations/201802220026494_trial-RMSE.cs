namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trialRMSE : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrialResults");
            AddColumn("dbo.TrialResults", "TrialId", c => c.Int(nullable: false));
            AddColumn("dbo.TrialResults", "RMSE", c => c.Double(nullable: false));
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm", "TrialId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TrialResults");
            DropColumn("dbo.TrialResults", "RMSE");
            DropColumn("dbo.TrialResults", "TrialId");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm" });
        }
    }
}
