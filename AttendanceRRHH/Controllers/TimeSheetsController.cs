﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL.Security;
using AttendanceRRHH.BLL;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager")]
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
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var departments = db.Departments
                .Where(w => w.IsActive && companies.Contains(w.CompanyId))
                .Select(s => new
                {
                    s.DepartmentId,
                    Name = s.Company.Name.Substring(0,3).ToUpper() + " - " + s.Name
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

                //var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

               /* var timesheets = db.TimeSheets.Include(i => i.Employee)
                    .Where(w => w.Date >= myDate && w.Date <= myDate);*/

                var timesheets = from t in db.TimeSheets
                                 join a in db.UserCompanies on t.Employee.Department.CompanyId equals a.CompanyId
                                 where t.Date >= myDate && t.Date <= myDate
                                    && a.User.UserName == User.Identity.Name
                                 select t;
                    
                //si selecciona todos en la lista
                if(departmentId > -1)
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
                    .OrderBy(o => o.EmployeeId)
                    .ToList()
                    .Select(s => new { s.TimeSheetId, FullName = s.Employee.FullName, s.EmployeeId, EmployeeCode = s.Employee.EmployeeCode, s.In, s.Out, s.IsManualIn, s.IsManualOut, DepartmentId = s.Employee.DepartmentId, DepartmentName = s.Employee.Department.Name, ShiftStartTime = s.ShiftTime.StartTime, ShiftEndTime = s.ShiftTime.EndTime, ProfileUrl = s.Employee.ProfileUrl });

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

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(w => companies.Contains(w.Department.CompanyId)), "EmployeeId", "EmployeeCode", timeSheet.EmployeeId);
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
                timeSheet.UpdatedAt = DateTime.Now;

                db.Entry(timeSheet).State = EntityState.Modified;                          
                db.SaveChanges();

                MyLogger.GetInstance.Info("Daily record was edited successfull, RecordId: " + timeSheet.ShiftTimeId+", EmployeeId: "+timeSheet.EmployeeId);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(w => companies.Contains(w.Department.CompanyId)), "EmployeeId", "EmployeeCode", timeSheet.EmployeeId);
            return PartialView("_Edit", timeSheet);
        }

        //// GET: TimeSheets/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TimeSheet timeSheet = db.TimeSheets.Find(id);

        //    if (timeSheet == null)
        //    {
        //        return HttpNotFound();
        //    }
           
        //    return PartialView("_Delete", timeSheet);
        //}

        //// POST: TimeSheets/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TimeSheet timeSheet = db.TimeSheets.Find(id);
        //    db.TimeSheets.Remove(timeSheet);
        //    db.SaveChanges();

        //    MyLogger.GetInstance.Info("Daily record was deleted successfull, RecordId: " + timeSheet.ShiftTimeId + ", EmployeeId: " + timeSheet.EmployeeId);

        //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
