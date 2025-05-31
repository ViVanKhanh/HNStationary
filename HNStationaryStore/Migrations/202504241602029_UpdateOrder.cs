namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerName", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "Phone", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "Address", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Address");
            DropColumn("dbo.Orders", "Phone");
            DropColumn("dbo.Orders", "CustomerName");
        }
    }
}
