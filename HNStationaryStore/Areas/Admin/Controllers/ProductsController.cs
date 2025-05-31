using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index(string SearchText, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(SearchText))
            {
                products = products.Where(x => x.ProductName.Contains(SearchText));
            }

            products = products.OrderByDescending(x => x.CreatedAt); // hoặc theo cách bạn muốn

            return View(products.ToPagedList(pageNumber, pageSize));
        }
        // Controller
        public ActionResult GetSubCategoriesByCategory(int categoryId)
        {
            // Lấy các danh mục con theo categoryId
            var subCategories = db.SubCategories
                                  .Where(sc => sc.CategoryID == categoryId)
                                  .ToList();

            // Trả về dữ liệu dưới dạng JSON (SelectListItem với Value và Text)
            var result = subCategories.Select(sc => new SelectListItem
            {
                Value = sc.SubCategoryID.ToString(),
                Text = sc.SubCategoryName
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            //ViewBag.CategoryList = new SelectList(db.ProductCategories.ToList(), "CategoryId", "CategoryName");
            ViewBag.ProductCategoryList = new SelectList(db.ProductCategories, "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, int? rDefault)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Xử lý hình ảnh
                    if (Images != null && Images.Count > 0)
                    {
                        for (int i = 0; i < Images.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(Images[i]))
                            {
                                bool isDefault = rDefault.HasValue && rDefault == i + 1; // Vì index bắt đầu từ 1 trong view

                                model.ProductImages.Add(new ProductImage
                                {
                                    ProductID = model.ProductID,
                                    ImageURL = Images[i]
                                });
                            }
                        }
                    }

                    // Thiết lập các giá trị khác
                    model.CreatedAt = DateTime.Now;
                    model.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.ProductName);

                    // Thêm vào database
                    db.Products.Add(model);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }

                // Nếu ModelState không valid, giữ lại dữ liệu đã chọn
                ViewBag.ProductCategoryList = new SelectList(db.ProductCategories, "CategoryId", "CategoryName", model.ProductCategoryID);
                return View(model);
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");

                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm sản phẩm. Vui lòng thử lại.");
                ViewBag.ProductCategoryList = new SelectList(db.ProductCategories, "CategoryId", "CategoryName", model.ProductCategoryID);
                return View(model);
            }
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load sản phẩm kèm hình ảnh và danh mục
            Product product = db.Products
                              .Include(p => p.ProductImages)
                              .Include(p => p.SubCategory)
                              .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Chuẩn bị dữ liệu cho dropdown
            ViewBag.ProductCategoryList = new SelectList(db.ProductCategories, "CategoryId", "CategoryName", product.ProductCategoryID);
            ViewBag.SubCategoryID = new SelectList(db.SubCategories.Where(sc => sc.CategoryID == product.ProductCategoryID), "SubCategoryID", "SubCategoryName", product.SubCategoryID);

            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, List<string> Images, int? rDefault)
        {
            if (ModelState.IsValid)
            {
                // Lấy sản phẩm từ database
                var product = db.Products
                              .Include(p => p.ProductImages)
                              .FirstOrDefault(p => p.ProductID == model.ProductID);

                if (product == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin cơ bản
                product.ProductName = model.ProductName;
                product.ProductCode = model.ProductCode;
                product.Manufacturer = model.Manufacturer;
                product.Description = model.Description;
                product.PurchasePrice = model.PurchasePrice;
                product.SalePrice = model.SalePrice;
                product.StockQuantity = model.StockQuantity;
                product.IsSale = model.IsSale;
                product.DiscountPercentage = model.DiscountPercentage;
                product.DiscountStartDate = model.DiscountStartDate;
                product.DiscountEndDate = model.DiscountEndDate;
                product.IsActived = model.IsActived;
                product.IsHot = model.IsHot;
                product.ProductCategoryID = model.ProductCategoryID;
                product.SubCategoryID = model.SubCategoryID;
                product.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.ProductName);

                // Xử lý hình ảnh
                if (Images != null && Images.Count > 0)
                {
                    // Xóa ảnh cũ
                    db.ProductImages.RemoveRange(product.ProductImages);

                    // Thêm ảnh mới
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(Images[i]))
                        {
                            product.ProductImages.Add(new ProductImage
                            {
                                ProductID = product.ProductID,
                                ImageURL = Images[i]
                                
                            });
                        }
                    }
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại dữ liệu dropdown
            ViewBag.ProductCategoryList = new SelectList(db.ProductCategories, "CategoryId", "CategoryName", model.ProductCategoryID);
            ViewBag.SubCategoryID = new SelectList(db.SubCategories.Where(sc => sc.CategoryID == model.ProductCategoryID), "SubCategoryID", "SubCategoryName", model.SubCategoryID);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            try
            {
                // Xóa ảnh sản phẩm từ server (nếu cần)
                if (product.ProductImages != null)
                {
                    foreach (var img in product.ProductImages)
                    {
                        var path = Server.MapPath(img.ImageURL);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ToggleActive(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            try
            {
                product.IsActived = !product.IsActived;
                db.SaveChanges();
                return Json(new { success = true, isActive = product.IsActived });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ToggleSale(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            try
            {
                product.IsSale = !product.IsSale;
                db.SaveChanges();
                return Json(new { success = true, isSale = product.IsSale });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult ToggleHot(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            product.IsHot = !product.IsHot;
            db.SaveChanges();

            return Json(new { success = true, message = "Cập nhật trạng thái nổi bật thành công" });
        }
    }
}