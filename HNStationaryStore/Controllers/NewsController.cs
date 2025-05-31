using HNStationaryStore.Models;
using Microsoft.Owin.Security.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: News
        public ActionResult Index()
        {
            var items = db.News.ToList();
            return View(items);
        }
        public ActionResult Detail(int id )
        {
            var item = db.News.Find(id);
            return View(item);
        }
        public ActionResult Partial_News()
        {
            var items = db.News.Take(3).ToList();
            return PartialView(items);
        }
    }
}