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

namespace AttendanceRRHH.Controllers
{
    public class HomeController2 : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public JsonResult GetDepartments()
        {
            var departments = db.Departments
                .Where(w => w.IsActive)
                .Select(s => new
                {
                    s.DepartmentId,
                    Name = s.Company.Name + " - " + s.Name
                });

            return Json(departments.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult HomeTopPanel()
        {
            return PartialView("_HomeTopPanel");
        }

        public ActionResult Index(string department, string date)
        {
            //DailyProcess d = new DailyProcess(db);
            //d.GenerateEmployeeTimeSheetByDate(new DateTime(2016, 02, 23));

            //ViewBag.department = new SelectList(db.Departments, "DepartmentId", "Name");

            var records = db.TimeSheets.Include(e => e.Employee).Include(s => s.ShiftTime);
            var mydate = DateTime.Now;
            int departmentId = 0;

            if (String.IsNullOrEmpty(department))
            {
                departmentId = db.Departments.FirstOrDefault().DepartmentId;
            }
            else
            {
                departmentId = Int32.Parse(department);
            }

            if(!String.IsNullOrEmpty(date))
            {
                mydate = DateTime.Parse(date);
            }

            if (departmentId < 0)
            {
                records = records
                    .Where(w => w.Date.Year == mydate.Year &&
                           w.Date.Month == mydate.Month &&
                           w.Date.Day == mydate.Day);
            }
            else
            {
                //get a specific department 
                records = from r in records
                          where r.Employee.DepartmentId == departmentId &&
                                r.Date.Year == mydate.Year &&
                                r.Date.Month == mydate.Month &&
                                r.Date.Day == mydate.Day
                          select r;

            }

            TimeSheetViewModel timesheet = new TimeSheetViewModel()
            {
                TimeSheetList = records.ToList()
            };

            return View(timesheet);
        }

        [HttpPost]
        public JsonResult Save(TimeSheetViewModel obj)
        {
            bool success = false;
            string message = "";

            if (ModelState.IsValid)
            {
                success = true;
                //db.Entry(obj.TimeSheetList).State = EntityState.Modified;

                foreach(var timesheet in obj.TimeSheetList)
                {
                    TimeSheet t = db.TimeSheets.Find(timesheet.TimeSheetId);
                    t.In = timesheet.In;
                    t.Out = timesheet.Out;
                    db.Entry(t).State = EntityState.Modified;
                }
                db.SaveChanges();
            }        

            return Json( new { success = success, message = message });
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