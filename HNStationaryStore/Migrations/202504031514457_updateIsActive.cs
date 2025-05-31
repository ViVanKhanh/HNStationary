namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsActived", c => c.Boolean(nullable: false));
            AddColumn("dbo.News", "IsActived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "IsActived");
            DropColumn("dbo.Products", "IsActived");
        }
    }
}
