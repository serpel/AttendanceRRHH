using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.Controllers
{
    public class TimeSheetsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TimeSheets
        public ActionResult Index()
        {
            return View();
        }

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
            if (date == null || department == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int departmentId = Int32.Parse(department);
                DateTime myDate = DateTime.Parse(date);

                var timesheets = db.TimeSheets.Include(i => i.Employee)
                    .Where(w => w.Date >= myDate && w.Date <= myDate);

                if (departmentId > 0)
                {
                    timesheets = timesheets
                        .Where(w => w.Employee.DepartmentId == departmentId);
                }

                var absences = (from t in timesheets
                                join e in db.Employees on t.EmployeeId equals e.EmployeeId
                                join ea in db.EmployeeAbsences on e.EmployeeId equals ea.EmployeeId
                                where myDate >= ea.StartDate && myDate <= ea.EndDate
                                select t).Distinct();

                if (absences.Count() > 0)
                {
                    timesheets = timesheets.Except(absences);
                }

                var list = timesheets
                    .ToList()
                    .Select(s => new { s.TimeSheetId, FullName = s.Employee.FullName, s.EmployeeId, EmployeeCode = s.Employee.EmployeeCode, s.In, s.Out, s.IsManualIn, s.IsManualOut, DepartmentId = s.Employee.DepartmentId, DepartmentName = s.Employee.Department.Name, ShiftStartTime = s.ShiftTime.StartTime, ShiftEndTime = s.ShiftTime.EndTime });

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: TimeSheets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode", timeSheet.EmployeeId);
            return PartialView("_Edit", timeSheet);
        }

        // POST: TimeSheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TimeSheetId,EmployeeId,ShiftTimeId,Date,In,Out,IsManualIn,IsManualOut,InsertedAt,UpdatedAt,IsActive")] TimeSheet timeSheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheet).State = EntityState.Modified;
                timeSheet.UpdatedAt = DateTime.Now;
                            
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode", timeSheet.EmployeeId);
            return PartialView("_Edit", timeSheet);
        }

        // GET: TimeSheets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            if (timeSheet == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", timeSheet);
        }

        // POST: TimeSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeSheet timeSheet = db.TimeSheets.Find(id);
            db.TimeSheets.Remove(timeSheet);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
