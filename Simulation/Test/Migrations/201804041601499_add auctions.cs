namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addauctions : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrialResults");
            AddColumn("dbo.TrialResults", "PushAuctionType", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TrialResults", "PullAcutionType", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAcutionType" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TrialResults");
            DropColumn("dbo.TrialResults", "PullAcutionType");
            DropColumn("dbo.TrialResults", "PushAuctionType");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm", "TrialId" });
        }
    }
}
