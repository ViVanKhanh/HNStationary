namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "SalePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "SalePrice");
            DropColumn("dbo.Products", "PurchasePrice");
        }
    }
}
