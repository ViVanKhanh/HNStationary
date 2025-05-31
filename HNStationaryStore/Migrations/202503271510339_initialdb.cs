namespace HNStationaryStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        CartItemID = c.Int(nullable: false, identity: true),
                        CartID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        PriceAtAdd = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Product_ProductID = c.Int(),
                    })
                .PrimaryKey(t => t.CartItemID)
                .ForeignKey("dbo.Products", t => t.Product_ProductID)
                .ForeignKey("dbo.Carts", t => t.CartID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.CartID)
                .Index(t => t.ProductID)
                .Index(t => t.Product_ProductID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        UserID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CartID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(maxLength: 20),
                        Address = c.String(),
                        RoleID = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleID)
                .Index(t => t.RoleID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderStatus = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Product_ProductID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID)
                .Index(t => t.Product_ProductID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 150),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountStartDate = c.DateTime(),
                        DiscountEndDate = c.DateTime(),
                        StockQuantity = c.Int(nullable: false),
                        SubCategoryID = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryID)
                .Index(t => t.SubCategoryID);
            
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        QuantityAvailable = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ImageURL = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.ImageID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryID = c.Int(nullable: false, identity: true),
                        SubCategoryName = c.String(nullable: false, maxLength: 100),
                        CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryID)
                .ForeignKey("dbo.ProductCategories", t => t.CategoryID)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                        PaymentMethod = c.String(nullable: false),
                        PaymentStatus = c.String(nullable: false),
                        TransactionID = c.String(maxLength: 100),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.TransactionID, unique: true);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        Position = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        NewsID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Content = c.String(nullable: false),
                        ImageURL = c.String(maxLength: 255),
                        AuthorID = c.Int(nullable: false),
                        Author_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.NewsID)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.News", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CartItems", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CartItems", "CartID", "dbo.Carts");
            DropForeignKey("dbo.Carts", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.Orders", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Payments", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Products", "SubCategoryID", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "CategoryID", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductImages", "ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "Product_ProductID", "dbo.Products");
            DropForeignKey("dbo.Inventories", "ProductID", "dbo.Products");
            DropForeignKey("dbo.CartItems", "Product_ProductID", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.News", new[] { "Author_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Payments", new[] { "TransactionID" });
            DropIndex("dbo.Payments", new[] { "OrderID" });
            DropIndex("dbo.SubCategories", new[] { "CategoryID" });
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            DropIndex("dbo.Inventories", new[] { "ProductID" });
            DropIndex("dbo.Products", new[] { "SubCategoryID" });
            DropIndex("dbo.OrderDetails", new[] { "Product_ProductID" });
            DropIndex("dbo.OrderDetails", new[] { "ProductID" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "RoleID" });
            DropIndex("dbo.Carts", new[] { "UserID" });
            DropIndex("dbo.CartItems", new[] { "Product_ProductID" });
            DropIndex("dbo.CartItems", new[] { "ProductID" });
            DropIndex("dbo.CartItems", new[] { "CartID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.News");
            DropTable("dbo.Category");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.Payments");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.SubCategories");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Inventories");
            DropTable("dbo.Products");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Orders");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Carts");
            DropTable("dbo.CartItems");
        }
    }
}
