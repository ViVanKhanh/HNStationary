using HNStationaryStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Roles
        public ActionResult Index()
        {
            var items = db.Roles.ToList();
            return View(items);
        }
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole model)
        {
            if(ModelState.IsValid)
            {
                var roleManage = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                roleManage.Create(model);
                return RedirectToAction("Index");
                
            }
            return View(model);
        }
        public ActionResult Edit(string id)  // sửa từ int sang string
        {
            var item = db.Roles.Find(id);
            if (item == null) return HttpNotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var role = roleManager.FindById(model.Id);
                if (role != null)
                {
                    role.Name = model.Name;
                    roleManager.Update(role);
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            return View(model);
        }

    }
}