namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TrialResults");
            AddColumn("dbo.TrialResults", "TotalContainers", c => c.Double(nullable: false));
            AlterColumn("dbo.TrialResults", "Tested", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TrialResults");
            AlterColumn("dbo.TrialResults", "Tested", c => c.Int(nullable: false));
            DropColumn("dbo.TrialResults", "TotalContainers");
            AddPrimaryKey("dbo.TrialResults", new[] { "Size", "StartUtil", "Change", "Algorithm", "Tested", "SchedulingAlgorithm" });
        }
    }
}
