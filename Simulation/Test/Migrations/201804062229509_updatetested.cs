namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetested : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("dbo.TrialResults");
            RenameColumn("dbo.TrialResults", "PullAcutionType", "PullAuctionType");
            RenameColumn("dbo.TrialResults", "Tested", "TestedPercent");
            //AlterColumn("dbo.TrialResults", "TestedPercent", c => c.Int());
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "TestedPercent", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAuctionType" });
        }
        
        public override void Down()
        {
            RenameColumn("dbo.TrialResults", "PullAuctionType", "PullAcutionType");
            AddColumn("dbo.TrialResults", "Tested", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.TrialResults");
            DropColumn("dbo.TrialResults", "TestedPercent");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm", "TrialId", "PushAuctionType", "PullAcutionType" });
        }
    }
}
