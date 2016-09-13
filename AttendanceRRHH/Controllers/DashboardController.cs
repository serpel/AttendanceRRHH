using AttendanceRRHH.DAL.Security;
using AttendanceRRHH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager, User")]

    public class DashboardController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            int totalActives = 0, totalInactives = 0, percent = 0;

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var employees = db.Employees.Where(w => companies.Contains(w.Department.CompanyId));

            if (employees != null)
            {
                totalActives = employees.Where(w => w.IsActive).Count();
                totalInactives = employees.Where(w => w.IsActive == false).Count();
                percent = (totalActives / (totalActives + totalInactives)) * 100;
            }

            ViewBag.TotalActiveEmployees = totalActives;
            ViewBag.TotalInactiveEmployees = totalInactives;
            ViewBag.Percent = percent;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}