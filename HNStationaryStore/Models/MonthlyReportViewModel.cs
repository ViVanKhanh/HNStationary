using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNStationaryStore.Models
{
    public class MonthlyReportViewModel
    {
        public int Month { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}