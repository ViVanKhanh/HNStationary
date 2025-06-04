using HNStationaryStore.Models;
using HNStationaryStore.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MenuTop()
        {
            var items = db.Categories.OrderBy(x => x.Position).ToList();
            return PartialView("_MenuTop", items);
        }
        public ActionResult MenuProductCategory()
        {
            var categories = db.ProductCategories.Include("SubCategories").ToList();
            return PartialView("_MenuProductCategory", categories);
        }
        public ActionResult MenuLeft()
        {
            var categories = db.ProductCategories.Include("SubCategories").ToList();
            return PartialView("_MenuLeft", categories);
        }
        public ActionResult DanhMucSanPham()
        {
            var items = db.ProductCategories.Take(4).ToList();
            return PartialView("DanhMucSanPham", items);
        }
        

    }
}