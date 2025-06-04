using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Index(int page = 1, int pageSize = 16, int? priceRange = null)
        {
            var items = db.Products.Where(p => p.IsActived).AsQueryable(); // lọc sản phẩm đang hoạt động

            // Lọc theo khoảng giá
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        items = items.Where(p => p.SalePrice < 50000);
                        break;
                    case 2:
                        items = items.Where(p => p.SalePrice >= 50000 && p.SalePrice <= 100000);
                        break;
                    case 3:
                        items = items.Where(p => p.SalePrice > 100000 && p.SalePrice <= 200000);
                        break;
                    case 4:
                        items = items.Where(p => p.SalePrice > 200000 && p.SalePrice <= 300000);
                        break;
                    case 5:
                        items = items.Where(p => p.SalePrice > 300000 && p.SalePrice <= 500000);
                        break;
                    case 6:
                        items = items.Where(p => p.SalePrice > 500000);
                        break;
                }
            }

            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedItems = items.OrderBy(x => x.ProductID)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PriceRange = priceRange;

            return View(pagedItems);
        }


        public ActionResult Detail(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult Partial_ItemById()
        {
            var items = db.Products.Where(x => x.IsHot).Take(12).ToList();
            return PartialView(items);
        }


        public ActionResult Partial_ItemSale()
        {
            DateTime now = DateTime.Now;

            var items = db.Products
                          .Where(x => x.IsSale
                                      && x.DiscountPercentage > 0
                                      && x.DiscountStartDate <= now
                                      && x.DiscountEndDate >= now)
                          .Take(10)
                          .ToList();

            return PartialView(items);
        }

        public ActionResult ProductBySubCategory(int id, int page = 1, int pageSize = 16, int? priceRange = null)
        {
            // Lấy subcategory và liên kết liên quan
            var subCategory = db.SubCategories
                                .Include("Products.ProductImages")
                                .Include("ProductCategory")
                                .FirstOrDefault(s => s.SubCategoryID == id);

            if (subCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryName = subCategory.ProductCategory.CategoryName;
            ViewBag.SubCategoryName = subCategory.SubCategoryName;
            ViewBag.SubCategoryID = id;
            ViewBag.PageSize = pageSize;
            ViewBag.PriceRange = priceRange;

            // Lọc sản phẩm đã kích hoạt
            var filteredProducts = subCategory.Products
                                              .Where(p => p.IsActived)
                                              .AsQueryable();

            // Lọc theo khoảng giá nếu có
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice < 50000);
                        break;
                    case 2:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice >= 50000 && p.SalePrice <= 100000);
                        break;
                    case 3:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 100000 && p.SalePrice <= 200000);
                        break;
                    case 4:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 200000 && p.SalePrice <= 300000);
                        break;
                    case 5:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 300000 && p.SalePrice <= 500000);
                        break;
                    case 6:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 500000);
                        break;
                }
            }

            filteredProducts = filteredProducts.OrderByDescending(p => p.IsHot)
                                               .ThenByDescending(p => p.CreatedAt);

            int totalItems = filteredProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedProducts = filteredProducts.Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedProducts);
        }



        public ActionResult ProductByCategory(int id, int page = 1, int pageSize = 16, int? priceRange = null)
        {
            var category = db.ProductCategories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryName = category.CategoryName;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.PriceRange = priceRange;

            var filteredProducts = db.Products
                                     .Include("ProductImages")
                                     .Where(p => p.ProductCategoryID == id && p.IsActived)
                                     .AsQueryable();

            // Lọc theo khoảng giá nếu có
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice < 50000);
                        break;
                    case 2:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice >= 50000 && p.SalePrice <= 100000);
                        break;
                    case 3:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 100000 && p.SalePrice <= 200000);
                        break;
                    case 4:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 200000 && p.SalePrice <= 300000);
                        break;
                    case 5:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 300000 && p.SalePrice <= 500000);
                        break;
                    case 6:
                        filteredProducts = filteredProducts.Where(p => p.SalePrice > 500000);
                        break;
                }
            }

            filteredProducts = filteredProducts
                               .OrderByDescending(p => p.IsHot)
                               .ThenByDescending(p => p.CreatedAt);

            int totalItems = filteredProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedProducts = filteredProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = totalPages;

            return View(pagedProducts);
        }


        public ActionResult _ProductReviews()
        {
           
            return PartialView();
        }

        public ActionResult _ReviewForm()
        {

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(ProductReview model)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Detail", "Product") });
            }
            if (!ModelState.IsValid)
            {
                TempData["ReviewError"] = "Vui lòng điền đầy đủ thông tin hợp lệ.";
                return RedirectToAction("Details", new { id = model.ProductID });
            }

            model.CreatedDate = DateTime.Now;
            db.ProductReviews.Add(model);
            db.SaveChanges();

            // Redirect về trang Details sau khi lưu thành công để reset form và không bị lỗi
            return RedirectToAction("Detail", new { id = model.ProductID });
        }

        [HttpGet]
        public ActionResult TimKiem(string query, int page = 1, int pageSize = 16)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.TuKhoa = "";
                return View("KetQuaTimKiem", new List<Product>());
            }

            var ketQua = db.Products
                           .Include("ProductImages")
                           .Where(sp => sp.IsActived && sp.ProductName.Contains(query))
                           .OrderByDescending(sp => sp.IsHot)
                           .ThenByDescending(sp => sp.CreatedAt)
                           .ToList();

            int totalItems = ketQua.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedKetQua = ketQua.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TuKhoa = query;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("TimKiem", pagedKetQua);
        }

        

    }
}