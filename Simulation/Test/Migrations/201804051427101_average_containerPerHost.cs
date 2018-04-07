namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class average_containerPerHost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialResults", "AverageContainerPerHost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialResults", "AverageContainerPerHost");
        }
    }
}
