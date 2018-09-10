namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class final : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrialResults");
            AddColumn("dbo.TrialResults", "TestedPercent", c => c.Int(nullable: false));
            AddColumn("dbo.TrialResults", "PullAuctionType", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TrialResults", "ContainerType", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TrialResults", "NetworkDelay", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrialResults", "AverageDownTime", c => c.Double(nullable: false));
            AddColumn("dbo.TrialResults", "SlaViolationsPercent", c => c.Double(nullable: false));
            AddColumn("dbo.TrialResults", "TotalCommunicatedData", c => c.Double(nullable: false));
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "TestedPercent", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAuctionType", "ContainerType", "NetworkDelay" });
            DropColumn("dbo.TrialResults", "Tested");
            DropColumn("dbo.TrialResults", "PullAcutionType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrialResults", "PullAcutionType", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TrialResults", "Tested", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.TrialResults");
            DropColumn("dbo.TrialResults", "TotalCommunicatedData");
            DropColumn("dbo.TrialResults", "SlaViolationsPercent");
            DropColumn("dbo.TrialResults", "AverageDownTime");
            DropColumn("dbo.TrialResults", "NetworkDelay");
            DropColumn("dbo.TrialResults", "ContainerType");
            DropColumn("dbo.TrialResults", "PullAuctionType");
            DropColumn("dbo.TrialResults", "TestedPercent");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAcutionType" });
        }
    }
}
