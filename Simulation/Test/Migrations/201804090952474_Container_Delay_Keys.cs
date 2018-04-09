namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Container_Delay_Keys : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrialResults");
            AddColumn("dbo.TrialResults", "ContainerType", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.TrialResults", "NetworkDelay", c => c.Boolean(nullable: false));
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "TestedPercent", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAuctionType", "ContainerType", "NetworkDelay" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TrialResults");
            DropColumn("dbo.TrialResults", "NetworkDelay");
            DropColumn("dbo.TrialResults", "ContainerType");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "TestedPercent", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAuctionType" });
        }
    }
}
