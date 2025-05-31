using HNStationaryStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using HNStationaryStore.Models.EF;
using PagedList;
using System.Collections.Generic;
using HNStationaryStore.Models.Commons;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin, employee"))]
    public class SubCategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/SubCategory
        public ActionResult Index(string SearchText, int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }

            // Lấy danh sách News (tất cả các bản ghi)
            IEnumerable<SubCategory> items = db.SubCategories.OrderByDescending(x => x.SubCategoryID);

            // Nếu có dữ liệu tìm kiếm, lọc theo từ khóa
            if (!string.IsNullOrEmpty(SearchText))
            {
                items = items.Where(x => x.SubCategoryName.Contains(SearchText));
            }

            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);

            ViewBag.pageSize = pageSize;
            ViewBag.page = page;

            return View(items);
        }
        // GET: Admin/SubCategory/Add
        public ActionResult Add()
        {
            // Truyền danh sách ProductCategories cho view để tạo dropdown
            ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/SubCategory/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SubCategory model)
        {
            if (ModelState.IsValid)
            {
                if (!db.ProductCategories.Any(c => c.CategoryId == model.CategoryID))
                {
                    ModelState.AddModelError("CategoryID", "CategoryID được chọn không tồn tại.");
                    ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName");
                    return View(model);
                }

                model.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.SubCategoryName); // <-- Thêm dòng này

                db.SubCategories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName");
            return View(model);
        }

        // GET: Admin/SubCategory/Edit/5
        public ActionResult Edit(int id)
        {
            // Lấy SubCategory theo id
            var subCategory = db.SubCategories.FirstOrDefault(s => s.SubCategoryID == id);
            if (subCategory == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy
            }

            // Truyền danh sách ProductCategories và SubCategory hiện tại vào view
            ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", subCategory.CategoryID);
            return View(subCategory);
        }

        // POST: Admin/SubCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubCategory model)
        {
            if (ModelState.IsValid)
            {
                if (!db.ProductCategories.Any(c => c.CategoryId == model.CategoryID))
                {
                    ModelState.AddModelError("CategoryID", "CategoryID được chọn không tồn tại.");
                    ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", model.CategoryID);
                    return View(model);
                }

                var existingSubCategory = db.SubCategories.FirstOrDefault(s => s.SubCategoryID == model.SubCategoryID);
                if (existingSubCategory != null)
                {
                    existingSubCategory.SubCategoryName = model.SubCategoryName;
                    existingSubCategory.CategoryID = model.CategoryID;
                    existingSubCategory.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.SubCategoryName); // <-- Thêm dòng này

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Không tìm thấy SubCategory.");
            }

            ViewBag.CategoryList = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.SubCategories.Find(id);
            if (item != null)
            {

                db.SubCategories.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
