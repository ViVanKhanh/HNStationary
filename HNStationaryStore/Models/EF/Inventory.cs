using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class Inventory
    {
        [Key, ForeignKey("Product")] // Sửa lại cách đặt ForeignKey
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
        public int QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }

        // Quan hệ: Một tồn kho thuộc một sản phẩm
        public virtual Product Product { get; set; }

    }
}