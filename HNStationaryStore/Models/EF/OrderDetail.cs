using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }
        [ForeignKey("Order")]
        public int OrderID { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }


        // Quan hệ: Một chi tiết đơn hàng thuộc một đơn hàng
        public virtual Order Order { get; set; }

        // Quan hệ: Một chi tiết đơn hàng thuộc một sản phẩm
        public virtual Product Product { get; set; }

        [NotMapped]
        [Display(Name = "Thành tiền")]
        public decimal Subtotal
        {
            get { return Quantity * UnitPrice; }
        }
    }
}