namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remoreitemfromorder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Orders", new[] { "UserID" });
            AlterColumn("dbo.Orders", "CustomerName", c => c.String());
            AlterColumn("dbo.Orders", "TypePayment", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "UserID");
            DropColumn("dbo.Orders", "OrderStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OrderStatus", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "UserID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Orders", "TypePayment", c => c.String());
            AlterColumn("dbo.Orders", "CustomerName", c => c.String(nullable: false));
            CreateIndex("dbo.Orders", "UserID");
            AddForeignKey("dbo.Orders", "UserID", "dbo.AspNetUsers", "Id");
        }
    }
}
