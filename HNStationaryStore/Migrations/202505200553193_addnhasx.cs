namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnhasx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Manufacturer", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Manufacturer");
        }
    }
}
