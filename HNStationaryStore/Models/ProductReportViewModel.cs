using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models
{
    public class ProductSalesInfo
    {
        public Product Product { get; set; }
        public int TotalSold { get; set; }
    }
    public class ProductReportViewModel
    {
        public List<ProductSalesInfo> TopSelling { get; set; }
        public List<ProductSalesInfo> LeastSelling { get; set; }
    }
}