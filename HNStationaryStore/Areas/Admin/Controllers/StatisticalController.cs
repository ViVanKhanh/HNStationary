using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // 1. Top 10 sản phẩm bán chạy (chỉ tính đơn đã giao)
            var topSelling = db.OrderDetails
                .Where(od => od.Order.OrderStatus == OrderStatus.Delivered) // Chỉ lấy đơn đã giao
                .GroupBy(od => od.Product)
                .Select(g => new ProductSalesInfo
                {
                    Product = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5)
                .ToList();

            // 2. Top 10 sản phẩm bán chậm (bao gồm cả sản phẩm chưa bán được)
            var leastSelling = (from p in db.Products
                                join od in db.OrderDetails on p.ProductID equals od.ProductID into odGroup
                                from odg in odGroup.DefaultIfEmpty()
                                where odg == null || odg.Order.OrderStatus == OrderStatus.Delivered
                                group odg by p into g
                                select new ProductSalesInfo
                                {
                                    Product = g.Key,
                                    TotalSold = g.Sum(x => x == null ? 0 : x.Quantity)
                                })
                                .OrderBy(x => x.TotalSold)
                                .Take(5)
                                .ToList();

            var model = new ProductReportViewModel
            {
                TopSelling = topSelling,
                LeastSelling = leastSelling
            };

            return View(model);
        }


        [HttpGet]
        [Route("~/Admin/RevenueReport/MonthlyReport/{year:int?}")]
        public ActionResult MonthlyReport(int? year)
        {
            // Debug: Kiểm tra năm nhận được
            System.Diagnostics.Debug.WriteLine($"Year received: {year}");

            year = year ?? DateTime.Now.Year;

            // Debug: Kiểm tra dữ liệu
            var orders = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered && o.CreatedAt.Year == year)
                .ToList();
            System.Diagnostics.Debug.WriteLine($"Found {orders.Count} orders for year {year}");

            var monthlyData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered && o.CreatedAt.Year == year)
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new MonthlyReportViewModel
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Month) // Thêm sắp xếp theo tháng
                .ToList();

            // Tạo danh sách năm có dữ liệu
            var availableYears = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .Select(o => o.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Nếu không có năm nào, thêm năm hiện tại
            if (!availableYears.Any())
            {
                availableYears.Add(DateTime.Now.Year);
            }

            ViewBag.SelectedYear = year;
            ViewBag.AvailableYears = availableYears;

            return View(monthlyData);
        }

        public ActionResult YearlyReport()
        {
            var yearlyData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .GroupBy(o => o.CreatedAt.Year)
                .Select(g => new YearlyReportViewModel
                {
                    Year = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .ToList();

            return View(yearlyData);
        }

        [HttpGet]
        public ActionResult CustomDateReport(DateTime? fromDate, DateTime? toDate)
        {
            // Đặt ngày mặc định nếu null
            fromDate = fromDate ?? DateTime.Now.AddMonths(-1);
            toDate = toDate ?? DateTime.Now;

            // Đảm bảo toDate bao gồm cả ngày đó (trọn vẹn 1 ngày)
            toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);

            var customData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered &&
                           o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .AsEnumerable() // Chuyển sang LINQ to Objects để sử dụng DateTime.Date
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new CustomDateReportViewModel
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.FromDate = fromDate.Value.Date; // Chỉ lấy phần ngày
            ViewBag.ToDate = toDate.Value.Date; // Chỉ lấy phần ngày

            return View(customData);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
