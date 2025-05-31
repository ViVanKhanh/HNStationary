using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Models.EF
{
    public class Product
    {
        public Product() {
            this.ProductImages = new HashSet<ProductImage>();
            this.OrderDetails = new HashSet<OrderDetail>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150)]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Nhà sản xuất không được để trống")]
        [StringLength(100)]
        public string Manufacturer { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        [Required(ErrorMessage = "Giá nhập sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ")]
        public decimal PurchasePrice { get; set; }
        [Required(ErrorMessage = "Giá bán sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ")]
        public decimal SalePrice { get; set; }
        [Required(ErrorMessage = "Product Code không được để trống")]
        [StringLength(50)]
        public string ProductCode { get; set; }
        public bool IsSale { get; set; }
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; } = 0;
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
        public int StockQuantity { get; set; }
        [ForeignKey("SubCategory")]
        public int SubCategoryID { get; set; }
        public bool IsActived { get; set; }
        public bool IsHot { get; set; }
        public string Alias { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("ProductCategory")]
        public int ProductCategoryID { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }


        // Tính toán giá sau giảm
        [NotMapped]
        [Display(Name = "Giá sau giảm")]
        public decimal DiscountedPrice
        {
            get
            {
                if (DiscountPercentage > 0 &&
                    DiscountStartDate.HasValue && DiscountEndDate.HasValue &&
                    DateTime.Now >= DiscountStartDate.Value && DateTime.Now <= DiscountEndDate.Value)
                {
                    return SalePrice * (1 - DiscountPercentage / 100);
                }
                return SalePrice;
            }
        }

        // Kiểm tra có đang giảm giá không
        [NotMapped]
        public bool IsOnSale
        {
            get
            {
                return DiscountPercentage > 0 &&
                       DiscountStartDate.HasValue && DiscountEndDate.HasValue &&
                       DateTime.Now >= DiscountStartDate.Value && DateTime.Now <= DiscountEndDate.Value;
            }
        }
    }
}