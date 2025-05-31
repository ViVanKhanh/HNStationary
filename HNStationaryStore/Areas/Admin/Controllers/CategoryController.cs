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
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Category
        public ActionResult Index()
        {
            var items = db.Categories;
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category model)
        {
            if(ModelState.IsValid)
            {
                model.IsActive = model.IsActive || true;
                model.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.Title);
                db.Categories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = db.Categories.Find(model.Id);
                if (existingCategory == null)
                {
                    return HttpNotFound();
                }
                existingCategory.Title = model.Title;
                existingCategory.Position = model.Position;
                existingCategory.IsActive = model.IsActive;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Categories.Find(id);
            if (item != null)
            {
                
                db.Categories.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new {success = false});
        }
    }
}