using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using HNStationaryStore.Models.EF;

namespace HNStationaryStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        [Phone]
        public string Phone { get; set; }

        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bỏ quy ước đặt tên số nhiều
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Cấu hình ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            // Cấu hình Category
            modelBuilder.Entity<ProductCategory>()
                .ToTable("ProductCategories")
                .HasKey(c => c.CategoryId);

            // Cấu hình SubCategory
            modelBuilder.Entity<SubCategory>()
                .ToTable("SubCategories")
                .HasKey(sc => sc.SubCategoryID);

            modelBuilder.Entity<SubCategory>()
                .HasRequired(s => s.ProductCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(s => s.CategoryID)
                .WillCascadeOnDelete(false);

            // Cấu hình Product
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.ProductID);

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.SubCategory) // Sửa từ SubCategories thành SubCategory
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubCategoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
               .HasOptional(p => p.Inventory)
               .WithRequired(i => i.Product)
               .WillCascadeOnDelete(true);


            // Cấu hình ProductImage
            modelBuilder.Entity<ProductImage>()
                .ToTable("ProductImages")
                .HasKey(pi => pi.ImageID);

            modelBuilder.Entity<ProductImage>()
                .HasRequired(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductID)
                .WillCascadeOnDelete(true);

            // Cấu hình Cart
            modelBuilder.Entity<Cart>()
                .ToTable("Carts")
                .HasKey(c => c.CartID);

            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.ApplicationUser)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserID)
                .WillCascadeOnDelete(false);

            // Cấu hình CartItem
            modelBuilder.Entity<CartItem>()
                .ToTable("CartItems")
                .HasKey(ci => ci.CartItemID);

            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductID)
                .WillCascadeOnDelete(false);

            // Cấu hình Order
            modelBuilder.Entity<Order>()
                .ToTable("Orders")
                .HasKey(o => o.OrderID);

            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Payment)
                .WithRequired(p => p.Order)
                .WillCascadeOnDelete(true);

            // Cấu hình OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .ToTable("OrderDetails")
                .HasKey(od => od.OrderDetailID);

            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductID)
                .WillCascadeOnDelete(false);

            // Cấu hình Payment
            modelBuilder.Entity<Payment>()
                .ToTable("Payments")
                .HasKey(p => p.OrderID);

            modelBuilder.Entity<Order>()
                   .HasOptional(o => o.Payment) // Order có thể có hoặc không có Payment
                   .WithRequired(p => p.Order)  // Payment bắt buộc phải có Order
                   .WillCascadeOnDelete(true);

            modelBuilder.Entity<Payment>()
                .Property(p => p.TransactionID)
                .HasMaxLength(100);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.TransactionID)
                .IsUnique();

            // Cấu hình Inventory
            modelBuilder.Entity<Inventory>()
                .ToTable("Inventories")
                .HasKey(i => i.ProductID);


            // Cấu hình News
            modelBuilder.Entity<News>()
                .ToTable("News")
                .HasKey(n => n.NewsID);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}