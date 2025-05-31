using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }
        [ForeignKey("Cart")]
        public int CartID { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ")]
        public decimal PriceAtAdd { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }

        // Tính thành tiền
        [NotMapped]
        [Display(Name = "Thành tiền")]
        public decimal Subtotal
        {
            get { return Quantity * PriceAtAdd; }
        }
    }
}