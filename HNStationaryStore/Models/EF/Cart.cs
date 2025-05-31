using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models.EF
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }


        public string UserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return CartItems?.Sum(ci => ci.Quantity * ci.PriceAtAdd) ?? 0;
            }
        }

    }
}