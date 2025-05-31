namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteRole : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Roles");
            DropIndex("dbo.AspNetUsers", new[] { "RoleID" });
            DropColumn("dbo.AspNetUsers", "RoleID");
            DropTable("dbo.Roles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            AddColumn("dbo.AspNetUsers", "RoleID", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "RoleID");
            AddForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Roles", "RoleId");
        }
    }
}
