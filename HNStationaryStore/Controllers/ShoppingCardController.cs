using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HNStationaryStore.Common;
using System.Configuration;
using Microsoft.AspNet.Identity;
using HNStationaryStore.Models.Payments;

namespace HNStationaryStore.Controllers
{
    public class ShoppingCardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ShoppingCard
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.OrderStatus = OrderStatus.Shipping; //đã thanh toán
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }
        public ActionResult CheckOut()
        {
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                ViewBag.CheckCard = card;
            }
            return View();
        }
        public ActionResult CheckOutSuccess()
        {
           
            return View();
        }
        public ActionResult Partial_Item_Card()
        {

            ShoppingCard card = (ShoppingCard)Session["Card"];
            bool isLoggedIn = Session["User"] != null;
            ViewBag.IsLoggedIn = isLoggedIn;
            if (card != null)
            {
                return PartialView(card.items);
            }
            return PartialView();
        }
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                return PartialView(card.items);
            }
            return PartialView();
        }

        public ActionResult ShowCount()
        {
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                return Json(new { Count = card.items.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult CheckOut(OrderViewModel req)
            {
                // Kiểm tra người dùng đã đăng nhập chưa
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("CheckOut", "ShoppingCard") });
                }

                var code = new { Success = false, Code = -1, Url = "" };
                if (!ModelState.IsValid)
                {
                    return View("CheckOut", req);
                }

                ShoppingCard cart = (ShoppingCard)Session["Card"];
                if (cart == null || cart.items.Count == 0)
                {
                    ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                    return View("CheckOut", req);
                }

                bool isEnoughStock = true;
                foreach (var item in cart.items)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductID == item.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", $"Sản phẩm với ID {item.ProductId} không tồn tại.");
                        isEnoughStock = false;
                    }
                    else if (product.StockQuantity < item.Quantity)
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{product.ProductName}' chỉ còn {product.StockQuantity} sản phẩm, bạn đã đặt {item.Quantity}.");
                        isEnoughStock = false;
                    }
                }

                if (!isEnoughStock)
                {
                    return View("CheckOut", req);
                }
                string userId = User.Identity.GetUserId();
                Order order = new Order
                {
                    CustomerName = req.CustomerName,
                    Phone = req.Phone,
                    Address = req.Address,
                    Email = req.Email,
                    TotalPrice = cart.items.Sum(x => x.Price * x.Quantity),
                    TypePayment = req.TypePayment,
                    CreatedAt = DateTime.Now,
                    Code = "DH" + new Random().Next(1000, 9999),
                    OrderStatus = OrderStatus.Success,
                    UserId = userId
                };

                foreach (var x in cart.items)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductID == x.ProductId);
                    if (product != null)
                    {
                        order.OrderDetails.Add(new OrderDetail
                        {
                            ProductID = x.ProductId,
                            Quantity = x.Quantity,
                            UnitPrice = x.Price
                        });

                        // Cập nhật lại số lượng trong kho
                        product.StockQuantity -= x.Quantity;
                    }
                }

                db.Orders.Add(order);
                db.SaveChanges();

                // Gửi email
                string strSanPham = "";
                decimal ThanhTien = 0;
                foreach (var sp in cart.items)
                {
                    strSanPham += "<tr>";
                    strSanPham += $"<td>{sp.ProductName}</td>";
                    strSanPham += $"<td>{sp.Quantity}</td>";
                    strSanPham += $"<td>{sp.TotalPrice.ToString("N0").Replace(",", ".")} VND</td>";
                    strSanPham += "</tr>";

                    ThanhTien += sp.Price * sp.Quantity;
                }

                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send2.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code)
                                                 .Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"))
                                                 .Replace("{{SanPham}}", strSanPham)
                                                 .Replace("{{TenKhachHang}}", order.CustomerName)
                                                 .Replace("{{Phone}}", order.Phone)
                                                 .Replace("{{Email}}", order.Email)
                                                 .Replace("{{DiaChiNhanHang}}", order.Address)
                                                 .Replace("{{ThanhTien}}", ThanhTien.ToString("N0").Replace(",", ".") + " VND")
                                                 .Replace("{{TongTien}}", ThanhTien.ToString("N0").Replace(",", ".") + " VND");

                HNStationaryStore.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentCustomer, req.Email);

                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send1.html"));
                contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code)
                                           .Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"))
                                           .Replace("{{SanPham}}", strSanPham)
                                           .Replace("{{TenKhachHang}}", order.CustomerName)
                                           .Replace("{{Phone}}", order.Phone)
                                           .Replace("{{Email}}", order.Email)
                                           .Replace("{{DiaChiNhanHang}}", order.Address)
                                           .Replace("{{ThanhTien}}", ThanhTien.ToString("N0").Replace(",", ".") + " VND")
                                           .Replace("{{TongTien}}", ThanhTien.ToString("N0").Replace(",", ".") + " VND");

                HNStationaryStore.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentAdmin, ConfigurationManager.AppSettings["EmailAdmin"]);

                cart.ClearCard();
            //var url = "";
            if (req.TypePayment == 2)
            {
                var url = UrlPayment(req.TypePaymentVN, order.Code);
                return Json(new
                {
                    Success = true,
                    Code = req.TypePayment,
                    Url = url,
                    Message = "Redirecting to payment gateway"
                });
            }

            return Json(new
            {
                Success = true,
                Code = 1,
                Message = "COD order placed successfully"
            });
            //return RedirectToAction("CheckOutSuccess");
        }


        public ActionResult Partial_CheckOut()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            // Tạo một OrderViewModel và map dữ liệu từ ApplicationUser
            var model = new OrderViewModel
            {
                CustomerName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email
            };

            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Partial_CheckOut(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                if (user != null)
                {
                    user.FullName = model.CustomerName;
                    user.Phone = model.Phone;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    db.SaveChanges();
                }
            }

            return PartialView(model); // Trả về model sau khi xử lý
        }

        [HttpPost]
        public ActionResult AddToCard(int id, int quantity)
        {
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.ProductID == id);

            if (checkProduct != null)
            {
                // Kiểm tra nếu số lượng yêu cầu lớn hơn số lượng tồn kho
                if (quantity > checkProduct.StockQuantity)
                {
                    code = new
                    {
                        success = false,
                        msg = "Số lượng sản phẩm yêu cầu vượt quá số lượng tồn kho.",
                        code = -2,
                        Count = 0
                    };
                    return Json(code);  // Trả về lỗi nếu số lượng vượt quá tồn kho
                }

                // Lấy giỏ hàng từ session hoặc khởi tạo mới
                ShoppingCard card = (ShoppingCard)Session["Card"];
                if (card == null)
                {
                    card = new ShoppingCard();
                }

                // Tạo sản phẩm thêm vào giỏ
                ShoppingCardItem item = new ShoppingCardItem
                {
                    ProductId = checkProduct.ProductID,
                    ProductName = checkProduct.ProductName,
                    ProductCategory = checkProduct.ProductCategory?.CategoryName,
                    ProductSubCategory = checkProduct.SubCategory?.SubCategoryName,
                    Alias = checkProduct.Alias,
                    Quantity = quantity
                };

                // Lấy ảnh đầu tiên (nếu có)
                var firstImage = checkProduct.ProductImages.FirstOrDefault();
                if (firstImage != null)
                {
                    item.ProductImg = firstImage.ImageURL;
                }

                // Ưu tiên sử dụng DiscountPrice nếu có, nếu không thì dùng SalePrice
                item.Price = checkProduct.DiscountedPrice > 0 ? checkProduct.DiscountedPrice : checkProduct.SalePrice;

                // Tính tổng tiền
                item.TotalPrice = item.Quantity * item.Price;

                // Thêm vào giỏ và lưu lại session
                card.AddToCard(item, quantity);
                Session["Card"] = card;

                // Trả kết quả về client
                code = new
                {
                    success = true,
                    msg = "Thêm sản phẩm vào giỏ hàng thành công",
                    code = 1,
                    Count = card.items.Count
                };
            }

            return Json(code);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                var checkProduct = card.items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    card.Remove(id);
                    code = new { success = true, msg = "", code = 1, Count = card.items.Count };
                }
            }

            return Json(code);
        }
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                // Tìm sản phẩm trong giỏ hàng
                var item = card.items.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                {
                    var checkProduct = db.Products.FirstOrDefault(x => x.ProductID == id);
                    if (checkProduct != null && quantity > checkProduct.StockQuantity)
                    {
                        // Trả về thông báo lỗi và giá trị số lượng ban đầu
                        return Json(new { Success = false, Message = "Số lượng sản phẩm yêu cầu vượt quá số lượng tồn kho.", OriginalQuantity = item.Quantity });
                    }

                    // Cập nhật lại số lượng
                    item.Quantity = quantity;
                    item.TotalPrice = item.Quantity * item.Price;

                    // Cập nhật giỏ hàng vào session
                    Session["Card"] = card;

                    return Json(new { Success = true });
                }
            }

            return Json(new { Success = false, Message = "Giỏ hàng không tồn tại." });
        }


        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCard card = (ShoppingCard)Session["Card"];
            if (card != null)
            {
                card.ClearCard();
                return Json(new {Success = true});
            }
            return Json(new { Success = false });
        }
        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalPrice * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}