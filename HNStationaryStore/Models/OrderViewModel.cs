using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }
        public int TypePayment { get; set; }
        public int TypePaymentVN { get; set; }
        public List<Order> Orders { get; set; }
    }
}