using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin, employee"))]
    public class ProductCategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ProductCategory
        public ActionResult Index()
        {
            var items = db.ProductCategories.ToList();
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                
                db.ProductCategories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var ProductCate = db.ProductCategories.Find(id);
            if (ProductCate == null)
            {
                return HttpNotFound();
            }
            return View(ProductCate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = db.ProductCategories.Find(model.CategoryId);
                if (existingCategory == null)
                {
                    return HttpNotFound();
                }
                existingCategory.CategoryName = model.CategoryName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var item = db.ProductCategories.Find(id);
            if (item != null)
            {
                db.ProductCategories.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
       


    }
}