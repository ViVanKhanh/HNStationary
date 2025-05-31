using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Controllers
{
    public class UserInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserInfo
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách đơn hàng của user
            var orders = db.Orders
                           .Where(o => o.UserId == userId)
                           .OrderByDescending(o => o.CreatedAt)
                           .ToList();

            // Tạo ViewModel và truyền dữ liệu
            var model = new OrderViewModel
            {
                CustomerName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email,
                Orders = orders
            };

            return PartialView(model); // Hoặc View(model) nếu không dùng PartialView
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserInfo(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.FullName = model.CustomerName;
                user.Phone = model.Phone;
                user.Address = model.Address;
                user.Email = model.Email;

                db.SaveChanges();
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int orderId)
        {
            var userId = User.Identity.GetUserId();
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId && o.UserId == userId);

            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.OrderStatus != OrderStatus.Success)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Không thể hủy đơn hàng ở trạng thái hiện tại.");
            }

            // ✅ Lấy danh sách chi tiết đơn hàng
            var orderDetails = db.OrderDetails.Where(od => od.OrderID == orderId).ToList();

            // ✅ Cộng lại số lượng sản phẩm vào kho
            foreach (var detail in orderDetails)
            {
                var product = db.Products.FirstOrDefault(p => p.ProductID == detail.ProductID);
                if (product != null)
                {
                    product.StockQuantity += detail.Quantity;
                }
            }

            // ✅ Cập nhật trạng thái đơn hàng
            order.OrderStatus = OrderStatus.Cancelled;

            db.SaveChanges();
            TempData["SuccessMessage"] = "Đơn hàng đã được hủy và số lượng sản phẩm đã được cập nhật.";

            return RedirectToAction("Index");
        }

    }
}