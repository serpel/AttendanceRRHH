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
    public class HomeController : BaseController
    {
        private AttendanceContext db = new AttendanceContext();

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

        public JsonResult GetTimeSheets(string department, string date)
        {             
            if (date == null || department == null) { 
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int departmentId = Int32.Parse(department);
                DateTime myDate = DateTime.Parse(date);

                var timesheets = db.TimeSheets.Include(i => i.Employee)
                    .Where(w => w.Date >= myDate && w.Date <= myDate);

                if ( departmentId > 0 )
                {
                    timesheets = timesheets
                        .Where(w => w.Employee.DepartmentId == departmentId);
                }

                var absences = (from t in timesheets
                                 join e in db.Employees on t.EmployeeId equals e.EmployeeId
                                 join ea in db.EmployeeAbsences on e.EmployeeId equals ea.EmployeeId
                                 where ea.StartDate >= myDate && ea.EndDate <= myDate
                                 select t).Distinct();

                if( absences.Count() > 0 )
                {
                    timesheets = timesheets.Except(absences);
                }

                var list = timesheets
                    .ToList()
                    .Select(s => new { FullName = s.Employee.FullName, s.EmployeeId, EmployeeCode = s.Employee.EmployeeCode, s.In, s.Out, s.IsManualIn, s.IsManualOut, DepartmentId = s.Employee.DepartmentId, DepartmentName = s.Employee.Department.Name, ShiftStartTime = s.ShiftTime.StartTime, ShiftEndTime = s.ShiftTime.EndTime });    

                return Json(list, JsonRequestBehavior.AllowGet);
            }              
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