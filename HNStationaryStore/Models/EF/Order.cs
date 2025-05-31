using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNStationaryStore.Models.EF
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            CreatedAt = DateTime.Now;
            OrderStatus = OrderStatus.Success; // Mặc định: Đặt hàng thành công
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        public string Code { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tổng tiền không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không hợp lệ")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        // Trạng thái đơn hàng - không cần nhập từ form, tự gán mặc định
        [Required]
        public OrderStatus OrderStatus { get; set; }

        public int TypePayment { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Payment Payment { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus
    {
        [Display(Name = "Đặt hàng thành công")]
        Success = 1,

        [Display(Name = "Đang vận chuyển")]
        Shipping = 2,

        [Display(Name = "Đã giao hàng")]
        Delivered = 3,

        [Display(Name = "Đã hủy")]
        Cancelled = 4
    }
}
