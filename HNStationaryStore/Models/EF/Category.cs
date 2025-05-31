using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(150)]
        public string Title { get; set; }
        [Required(ErrorMessage = "vị trí không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Vị trí phải là số nguyên dương")]
        public int Position { get; set; }

        public bool IsActive { get; set; } = true;
        public string Alias { get; set; }
    }
}