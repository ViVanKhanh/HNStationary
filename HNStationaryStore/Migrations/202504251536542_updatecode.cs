namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Code");
        }
    }
}
