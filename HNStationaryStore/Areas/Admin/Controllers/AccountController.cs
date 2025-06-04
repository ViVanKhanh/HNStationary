using HNStationaryStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace HNStationaryStore.Areas.Admin.Controllers
{
    [Authorize(Roles = ("admin"))]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin/Account

        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var items = db.Users.ToList();
            var userRoles = new List<UserListViewModel>();

            foreach (var user in items)
            {
                var roleId = user.Roles.FirstOrDefault()?.RoleId;
                var role = roleId != null ? db.Roles.Find(roleId)?.Name : "Không có quyền";

                userRoles.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Address = user.Address,
                    Email = user.Email,
                    Role = role,
                    IsActive = user.IsActive,
                });
            }

            var pagedList = userRoles.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }


        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var user = UserManager.FindById(id);
            if (user == null)
                return HttpNotFound();

            var roles = db.Roles.Select(r => r.Name).ToList(); // Lấy danh sách role name từ db
            var userRoles = UserManager.GetRoles(user.Id);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email,
                Role = userRoles.FirstOrDefault()
            };

            ViewBag.Role = new SelectList(roles, model.Role); // Tạo SelectList, set selected là role hiện tại

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = UserManager.FindById(model.Id);
            if (user == null)
                return HttpNotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.Phone;
            user.Address = model.Address;
            user.Email = model.Email;

            var result = UserManager.Update(user);

            if (result.Succeeded)
            {
                var currentRoles = UserManager.GetRoles(user.Id);
                if (!currentRoles.Contains(model.Role))
                {
                    UserManager.RemoveFromRoles(user.Id, currentRoles.ToArray());
                    UserManager.AddToRole(user.Id, model.Role);
                }

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ToggleStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, message = "Id không hợp lệ" });

            var user = UserManager.FindById(id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy user" });

            // Giả sử bạn có trường IsActive trên ApplicationUser
            user.IsActive = !user.IsActive;  // Đổi trạng thái

            var result = UserManager.Update(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, isActive = user.IsActive });
            }
            else
            {
                return Json(new { success = false, message = "Cập nhật trạng thái thất bại" });
            }
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm người dùng theo username
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(model);
            }

            // Kiểm tra trạng thái hoạt động
            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa.");
                return View(model);
            }

            // Nếu hoạt động, tiến hành đăng nhập
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Đăng nhập không hợp lệ.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Address = model.Address,
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, model.Role);
                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


    }
}