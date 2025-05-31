using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class SubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCategoryID { get; set; }
        [Required(ErrorMessage = "Tên danh mục con không được để trống")]
        [StringLength(100)]
        public string SubCategoryName { get; set; }
        [ForeignKey("ProductCategory")]
        public int CategoryID { get; set; }
        public string Alias { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}