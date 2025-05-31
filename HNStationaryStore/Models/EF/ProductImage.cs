using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = ("Đường dẫn ảnh không được để trống"))]
        [StringLength(250, ErrorMessage = ("Đường dẫn ảnh không vượt quá 255 ký tự"))]
        public string ImageURL { get; set; }

        // Quan hệ: Một ảnh thuộc một sản phẩm
        public virtual Product Product { get; set; }
    }
}