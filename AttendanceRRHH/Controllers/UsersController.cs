using AttendanceRRHH.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AttendanceRRHH.Controllers
{
    public class UsersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

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

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUsers() {

            var result = db.Users
                        .Select(s => new { s.Id, s.UserName, s.Email, s.EmailConfirmed });

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(string id)
        {
            var user = UserManager.FindById(id);
            string [] selectedValues = user.Roles.Select(x => x.RoleId).ToArray();

            ViewBag.Roles = new MultiSelectList(db.Roles.ToList(), "Id", "Name", null, selectedValues);
            ViewBag.Companies = new MultiSelectList(db.Companies.ToList(), "CompanyId", "Name");

            UserViewModel userV = new UserViewModel() { Id = user.Id, Email = user.Email };

            return PartialView("_Edit", userV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);

                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var rolesToDelete = user.Roles.Select(s => s.RoleId).Except(model.Roles).ToList();
                var rolesToAdd = model.Roles.Except(user.Roles.Select(s => s.RoleId)).ToList();

                if (rolesToDelete.Count > 0)
                {
                    string[] roles = db.Roles
                        .Where(w => rolesToDelete.Contains(w.Id))
                        .Select(s => s.Name)
                        .ToArray();

                    await UserManager.RemoveFromRolesAsync(user.Id, roles);
                }

                if (rolesToAdd.Count > 0)
                {
                    string[] newRoles = db.Roles
                        .Where(w => rolesToAdd.Contains(w.Id))
                        .Select(s => s.Name)
                        .ToArray();

                    await UserManager.AddToRolesAsync(user.Id, newRoles);
                }

                List<string> companies = new List<string>();
                //TODO: save company user

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Id", "Name");
            ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");

            return PartialView("_Edit", model);
        }

    }
}