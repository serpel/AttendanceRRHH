using AttendanceRRHH.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using AttendanceRRHH.DAL.Security;
using AttendanceRRHH.BLL;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin")]
    public class RolesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRoles()
        {
            var result = db.Roles
                .Select(s => new { s.Id, s.Name });

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var role = new IdentityRole();
            return PartialView("_Create", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();

                MyLogger.GetInstance.Info("Role was created successfull: " + role.Name);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Create", role);
        }

        public ActionResult Edit(int id)
        {
            var role = db.Roles.Find(id);
            return PartialView("_Edit", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                MyLogger.GetInstance.Info("Role was edited successfull: " + role.Name);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_Edit", role);
        }
    }
}