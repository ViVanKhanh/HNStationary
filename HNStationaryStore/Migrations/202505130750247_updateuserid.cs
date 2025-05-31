namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateuserid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "UserId", c => c.String());
            AddColumn("dbo.Orders", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "ApplicationUser_Id");
            AddForeignKey("dbo.Orders", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Orders", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Orders", "ApplicationUser_Id");
            DropColumn("dbo.Orders", "UserId");
        }
    }
}
