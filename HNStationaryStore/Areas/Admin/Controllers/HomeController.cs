using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "admin, employee")]
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}