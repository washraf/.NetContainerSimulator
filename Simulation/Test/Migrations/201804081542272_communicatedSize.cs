namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class communicatedSize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrialResults", "TotalCommunicatedData", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrialResults", "TotalCommunicatedData");
        }
    }
}
