using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL;
using AttendanceRRHH.BLL;
using AttendanceRRHH.Helpers;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager, User")]
    public class HomeController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult HomeTopPanel()
        {
            return PartialView("_HomeTopPanel");
        }

        public ActionResult Index()
        {
            int totalActives = (db.Employees
                .Where(w => w.IsActive == true)).Count();
            
            int totalInactives = (db.Employees
                .Where(w => w.IsActive == false)).Count();

            ViewBag.TotalActiveEmployees = totalActives;
            ViewBag.TotalInactiveEmployees = totalInactives;

            ViewBag.Percent = (totalActives/(totalActives + totalInactives)) * 100;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

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