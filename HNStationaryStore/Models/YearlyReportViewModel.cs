using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models
{
    public class YearlyReportViewModel
    {
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }

        public decimal Profit { get; set; }
    }
}