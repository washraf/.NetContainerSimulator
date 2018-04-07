namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SLaViolationPercent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialResults", "SlaViolationsPercent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialResults", "SlaViolationsPercent");
        }
    }
}
