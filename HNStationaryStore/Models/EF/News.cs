using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Models.EF
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewsID { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        [StringLength(200, ErrorMessage = "Mô tả không vượt quá 200 ký tự")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Nội dung không được để trống")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Content { get; set; }
        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không vượt quá 255 ký tự")]
        public string Image { get; set; }
        public DateTime CreateDate { get; set; }
        public string Alias { get; set; }
        public bool IsActived { get; set; }

        // Quan hệ: Một tin tức thuộc một người dùng (admin/staff)
        public virtual ApplicationUser Author { get; set; }
    }
}