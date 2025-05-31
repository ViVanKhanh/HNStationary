using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models
{
    public class ShoppingCard
    {
        public List<ShoppingCardItem> items { get; set; }
        public ShoppingCard()
        {
            this.items = new List<ShoppingCardItem>();
        }

        public void AddToCard(ShoppingCardItem item, int Quantity)
        {
            var checkExists = items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExists != null)
            {
                checkExists.Quantity = Quantity;
                checkExists.TotalPrice = checkExists.Price * checkExists.Quantity;
            }
            else
            {
                items.Add(item);
            }
        }

        public void Remove(int id)
        {
            var checkExists = items.SingleOrDefault(x => x.ProductId ==  id);
            if (checkExists != null)
            {
                items.Remove(checkExists);
            }
        }
        public void UpdateQuantity (int id, int quantity)
        {
            var checkExists = items.SingleOrDefault(x => x.ProductId == id);
            if (checkExists != null)
            {
                checkExists.Quantity = quantity;
                checkExists.TotalPrice = checkExists.Price * checkExists.Quantity;
            }
        }

        public decimal GetTotalPrice()
        {
            return items.Sum(x => x.TotalPrice);
        }

        public decimal GetTotalQuantity()
        {
            return items.Sum(x => x.Quantity);
        }
        public void ClearCard()
        {
            items.Clear();
        }
    }

    public class ShoppingCardItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string Alias { get; set; }
        public string ProductSubCategory { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}