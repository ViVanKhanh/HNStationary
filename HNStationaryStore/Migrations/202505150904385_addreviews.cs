namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addreviews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductReview",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ReviewerName = c.String(nullable: false, maxLength: 100),
                        ReviewerEmail = c.String(),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductReview", "ProductID", "dbo.Products");
            DropIndex("dbo.ProductReview", new[] { "ProductID" });
            DropTable("dbo.ProductReview");
        }
    }
}
