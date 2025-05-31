using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class Payment
    {
        [Key, ForeignKey("Order")] // Sử dụng OrderID làm cả PK và FK
        public int OrderID { get; set; }
        [Required(ErrorMessage = "Phương thức thanh toán không được để trống")]
        public string PaymentMethod { get; set; }
        [Required(ErrorMessage = "Trạng thái thanh toán không được để trống")]
        public string PaymentStatus { get; set; }
        [StringLength(100, ErrorMessage = "Mã giao dịch không vượt quá 100 ký tự")]
        public string TransactionID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Quan hệ: Một thanh toán thuộc một đơn hàng
        public virtual Order Order { get; set; }

        // Danh sách phương thức thanh toán

        public static readonly List<string> MethodList = new List<string>
        {
             "Credit Card", "Bank Transfer", "COD", "Paypal"
        };
        // Danh sách trạng thái thanh toán
        public static readonly List<string> StatusList = new List<string>
        {
            "Pending", "Completed", "Failed", "Refunded"
        };
    }
}