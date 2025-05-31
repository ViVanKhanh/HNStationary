using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin, employee"))]
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/News
        public ActionResult Index(string SearchText, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }

            // Lấy danh sách News (tất cả các bản ghi)
            IEnumerable<News> items = db.News.OrderByDescending(x => x.NewsID);

            // Nếu có dữ liệu tìm kiếm, lọc theo từ khóa
            if (!string.IsNullOrEmpty(SearchText))
            {
                items = items.Where(x => x.Alias.Contains(SearchText) || x.Title.Contains(SearchText));
            }

            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);

            ViewBag.pageSize = pageSize;
            ViewBag.page = page;

            return View(items);
        }


        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(News model)
        {
            if(ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.Title);
                db.News.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model)
        {
            if (ModelState.IsValid)
            {
                model.Alias = HNStationaryStore.Models.Commons.Filter.FilterChar(model.Title);
                db.News.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.News.Find(id);
            if(item != null)
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new {success = true});
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                item.IsActived = !item.IsActived;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActived});
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                foreach (var item in items)
                {
                    var obj = db.News.Find(Convert.ToInt32(item));
                    if (obj != null)
                    {
                        db.News.Remove(obj);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}