using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class ProductReview
    {
        public int Id { get; set; }

        public int ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string ReviewerName { get; set; }

        [EmailAddress]
        public string ReviewerEmail { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; }
    }
}