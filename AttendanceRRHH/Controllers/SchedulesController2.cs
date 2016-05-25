using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace AttendanceRRHH.Controllers
{
    public class SchedulesController2 : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Schedule
        public ActionResult Index()
        {           
            return View();
        }

        //public ActionResult GetSchedules(string date)
        //{
        //    var myDate = DateTime.Parse(date);

        //    var result = from s in db.Schedules.ToList()
        //                 where (s.StartDate.DayOfYear / 7) == (myDate.DayOfYear / 7)
        //                 group s by s.EmployeeId into g
        //                 select new { g.Key, Monday = g.Select( a => a.) }
               

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

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