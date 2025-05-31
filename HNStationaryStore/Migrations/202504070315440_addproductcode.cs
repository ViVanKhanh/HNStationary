namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addproductcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductCode", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ProductCode");
        }
    }
}
