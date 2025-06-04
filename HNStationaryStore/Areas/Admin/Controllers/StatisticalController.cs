using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin, employee"))]
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
            year = year ?? DateTime.Now.Year;

            var monthlyData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered && o.CreatedAt.Year == year)
                .SelectMany(o => o.OrderDetails.Select(od => new
                {
                    Month = o.CreatedAt.Month,
                    Revenue = od.UnitPrice * od.Quantity, // Sử dụng UnitPrice từ OrderDetail
                    Profit = (od.UnitPrice - od.Product.PurchasePrice) * od.Quantity // Tính lợi nhuận dựa trên UnitPrice
                }))
                .GroupBy(x => x.Month)
                .Select(g => new MonthlyReportViewModel
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue),
                    Profit = g.Sum(x => x.Profit),
                    OrderCount = db.Orders.Count(o => o.OrderStatus == OrderStatus.Delivered &&
                                                     o.CreatedAt.Year == year &&
                                                     o.CreatedAt.Month == g.Key)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Tạo danh sách năm có dữ liệu
            var availableYears = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .Select(o => o.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            if (!availableYears.Any())
            {
                availableYears.Add(DateTime.Now.Year);
            }

            ViewBag.SelectedYear = year;
            ViewBag.AvailableYears = availableYears;

            return View(monthlyData);
        }
        public ActionResult MonthlyOrders(int year, int month, int page = 1, int pageSize = 10)
        {
            var query = db.Orders
                .Where(o => o.CreatedAt.Year == year && o.CreatedAt.Month == month && o.OrderStatus == OrderStatus.Delivered)
                .OrderByDescending(o => o.CreatedAt);

            var totalItems = query.Count();
            var orders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = month;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(orders);
        }
        public ActionResult YearlyReport()
        {
            var yearlyData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .GroupBy(o => o.CreatedAt.Year)
                .Select(g => new YearlyReportViewModel
                {
                    Year = g.Key,
                    TotalRevenue = g.SelectMany(o => o.OrderDetails)
                                    .Sum(od => od.UnitPrice * od.Quantity), // Tính tổng từ OrderDetails
                    OrderCount = g.Count(),
                    // Nếu cần tính lợi nhuận
                    Profit = g.SelectMany(o => o.OrderDetails)
                             .Sum(od => (od.UnitPrice - od.Product.PurchasePrice) * od.Quantity)
                })
                .OrderByDescending(x => x.Year)
                .ToList();

            return View(yearlyData);
        }

        [HttpGet]
        public ActionResult CustomDateReport(DateTime? fromDate, DateTime? toDate)
        {
            // Xử lý ngày mặc định
            fromDate = fromDate ?? DateTime.Now.AddMonths(-1);
            toDate = toDate ?? DateTime.Now;

            // Đặt toDate đến cuối ngày (23:59:59)
            toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);

            var customData = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered &&
                           o.CreatedAt >= fromDate &&
                           o.CreatedAt <= toDate)
                .SelectMany(o => o.OrderDetails.Select(od => new
                {
                    Date = DbFunctions.TruncateTime(o.CreatedAt),
                    Revenue = od.UnitPrice * od.Quantity, // Sử dụng UnitPrice thay vì Product.SalePrice
                    Profit = (od.UnitPrice - od.Product.PurchasePrice) * od.Quantity // Tính lợi nhuận chính xác
                }))
                .GroupBy(x => x.Date)
                .Select(g => new CustomDateReportViewModel
                {
                    Date = g.Key.Value,
                    TotalRevenue = g.Sum(x => x.Revenue),
                    Profit = g.Sum(x => x.Profit),
                    OrderCount = db.Orders.Count(o => o.OrderStatus == OrderStatus.Delivered &&
                                                    DbFunctions.TruncateTime(o.CreatedAt) == g.Key)
                })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.FromDate = fromDate.Value.Date;
            ViewBag.ToDate = toDate.Value.Date;

            return View(customData);
        }


        // Trong Controller
        public ActionResult DailyOrders(DateTime date, int page = 1, int pageSize = 10)
        {
            var query = db.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered &&
                           DbFunctions.TruncateTime(o.CreatedAt) == date.Date)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .OrderByDescending(o => o.CreatedAt);

            var totalItems = query.Count();
            var orders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.ReportDate = date.ToString("dd/MM/yyyy");
            ViewBag.TotalRevenue = query.Sum(o => o.TotalPrice);
            ViewBag.OrderCount = totalItems;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(orders);
        }
    }
}
