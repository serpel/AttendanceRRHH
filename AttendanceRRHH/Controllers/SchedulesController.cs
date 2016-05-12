using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using Newtonsoft.Json;

namespace AttendanceRRHH.Controllers
{
    public class SchedulesController : BaseController
    {
        private AttendanceContext db = new AttendanceContext();

        // GET: Schedule
        public ActionResult Index()
        {           
            return View();
        }

        public JsonResult Search(string employee)
        {
            bool success = false;
            string message = "EmployeeId is empty";

            if (!String.IsNullOrEmpty(employee))
            {
                try
                {
                    int employeeId = Int32.Parse(employee);

                    var schedules = db.Schedules
                        .Where(w => w.EmployeeId == employeeId)
                        .Select(s => new
                        {
                            id = s.ScheduleId,
                            title = s.Shift.Name,
                            start = s.StartDate,
                            end = s.EndDate,
                            url =  s.ShiftId,
                            allDay = false                            
                        });

                    success = true;
                    message = "";

                    return Json(new { success = success, message = message, data = schedules.ToList() }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e) { message = e.Message; }

                return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCompanies()
        {
            var companies = db.Companies
                .Where(w => w.IsActive)
                .Select(s => new
                {
                    s.CompanyId,
                    s.Name
                });

            return Json(companies.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartments(string company)
        {
            int companyId = Int32.Parse(company);
            var departments = db.Departments
                .Where(w => w.IsActive && w.CompanyId == companyId)
                .Select(s => new
                {
                    s.DepartmentId,
                    s.Name
                });

            return Json(departments.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(string department)
        {
            int departmentId = Int32.Parse(department);
            var employees = db.Employees
                .Where(w => w.IsActive && w.DepartmentId == departmentId)
                .Select(s => new
                {
                    s.EmployeeId,
                    s.FirstName,
                    s.LastName,
                    s.EmployeeCode
                });

            return Json(employees.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventsByEmployee(string employee)
        {
            if (!string.IsNullOrEmpty(employee))
            {
                int employeeId = Int32.Parse(employee);

                var schedules = db.Schedules
                    //.Where(w => w.StartDate >= startdate && w.EndDate <= enddate)
                    .Select(s => new
                    {
                        id = s.ScheduleId,
                        title = s.Shift.Name,
                        start = s.StartDate,
                        end = s.EndDate,
                        allDay = false
                    });

                return Json(schedules.ToList(), JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private void DeleteSchedulesByMonth(DateTime date, int employeeId)
        {
            var schedules = db.Schedules
                            .Where(w => w.StartDate.Year == date.Year &&
                                        w.StartDate.Month == date.Month &&
                                        w.EmployeeId == employeeId);

            db.Schedules.RemoveRange(schedules);
            db.SaveChanges();
        }

        public JsonResult Save(string json, string date, string employee)
        {
            bool success = false;
            string message = "Employee is empty";

            try
            {
                if (!string.IsNullOrEmpty(employee))
                {
                    int employeeId = Int32.Parse(employee);
                    var events = JsonConvert.DeserializeObject<IEnumerable<Schedule>>(json);

                    List<Schedule> eventsToCreate = new List<Schedule>();

                    foreach (Schedule s in events)
                    {
                        s.InsertedAt = DateTime.Now;
                        s.UpdatedAt = DateTime.Now;
                        s.EmployeeId = employeeId;

                        if (s.ScheduleId > 0)
                        {
                            Schedule tmp = db.Schedules.Find(s.ScheduleId);
                            tmp.StartDate = s.StartDate;
                            tmp.EndDate = s.EndDate;
                            tmp.UpdatedAt = DateTime.Now;
                            tmp.ShiftId = s.ShiftId;

                            db.Entry(tmp).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {                           
                            eventsToCreate.Add(s);
                        }       
                    }

                    //DeleteSchedulesByMonth(DateTime.Parse(date), employeeId);
                    db.Schedules.AddRange(eventsToCreate);
                    db.SaveChanges();
                    success = true;
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new { success = success, message = message });
        }

        public ActionResult CalendarLeftPanel()
        {
            var shifts = db.Shifts.
                Where(w => w.IsActive == true);

            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "CodeAndFullName");

            return PartialView("_CalendarLeftPanel", shifts.ToList());
        }

        public ActionResult CalendarTopPanel()
        {
            return PartialView("_CalendarTopPanel");
        }
    }
}