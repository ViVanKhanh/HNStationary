namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsSale", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsSale");
        }
    }
}
