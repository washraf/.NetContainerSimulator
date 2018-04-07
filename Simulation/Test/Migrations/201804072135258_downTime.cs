namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class downTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialResults", "AverageDownTime", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialResults", "AverageDownTime");
        }
    }
}
