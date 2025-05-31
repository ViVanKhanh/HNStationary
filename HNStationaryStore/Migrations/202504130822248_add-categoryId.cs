namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcategoryId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "ProductCategoryID");
            AddForeignKey("dbo.Products", "ProductCategoryID", "dbo.ProductCategories", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ProductCategoryID", "dbo.ProductCategories");
            DropIndex("dbo.Products", new[] { "ProductCategoryID" });
            DropColumn("dbo.Products", "ProductCategoryID");
        }
    }
}
