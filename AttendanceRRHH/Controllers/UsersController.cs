using AttendanceRRHH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceRRHH.Controllers
{
    public class UsersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
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


    }
}