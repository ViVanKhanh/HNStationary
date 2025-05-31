using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin, employee"))]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(string SearchText, int? page)
        {
            var items = db.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(SearchText))
            {
                items = items.Where(x => x.Code.Contains(SearchText));
            }

            items = items.OrderByDescending(x => x.CreatedAt);

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult UpdateStatus(int id, int status)
        {
            // Kiểm tra và log id và status
            Console.WriteLine($"ID: {id}, Status: {status}");

            var order = db.Orders.Find(id); // Tìm đơn hàng theo ID

            if (order != null)
            {
                order.OrderStatus = (OrderStatus)status; // Cập nhật trạng thái đơn hàng
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return Json(new { success = true }); // Trả về kết quả thành công
            }

            return Json(new { success = false, message = "Đơn hàng không tồn tại" });
        }

    }
}