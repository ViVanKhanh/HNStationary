namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsHot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsHot", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsHot");
        }
    }
}
