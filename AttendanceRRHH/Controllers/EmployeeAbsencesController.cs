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
    public class EmployeeAbsencesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult GetEmployeeAbsences()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var employeeAbsences = from s in db.EmployeeAbsences
                                   join e in db.Employees on s.EmployeeId equals e.EmployeeId
                                   where companies.Contains(e.Department.CompanyId)
                                   select new { s.EmployeeAbsenceId, Absence = s.Absence.Name, Employee = e.FirstName + " " + e.LastName, s.StartDate, s.EndDate, s.Comment };

            return Json(employeeAbsences.ToList().Select(s => new { s.EmployeeAbsenceId, s.Absence, s.Employee, StartDate = s.StartDate.ToShortDateString(), EndDate = s.EndDate.ToShortDateString(), s.Comment }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAbsences()
        {
            var absences = db.Absences
                .ToList()
                .Select(s => new { s.AbsenceId, s.Name, s.IsActive });

            return Json(absences, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployees(string searchTerm, int pageSize, int? pageNum)
        {
            if (searchTerm == null)
                return Json(new { Data = new Select2PagedResult() { Results = null, Total = 0 } }, JsonRequestBehavior.AllowGet);

            if (pageNum == null)
                pageNum = 1;

            var employeeAbsences = db.Employees
                .Where(w => w.FirstName.Contains(searchTerm) ||
                       w.LastName.Contains(searchTerm) ||
                       w.EmployeeCode.Contains(searchTerm))
                .OrderBy(o => o.EmployeeId)
                .Skip(pageSize * ((int)pageNum -1))
                .Take(pageSize)
                .ToList()
                .Select(s => new { s.EmployeeId, Code = s.EmployeeCode, Name = s.FullName });

            int count = employeeAbsences.Count();

            List<Select2Result> pagedAttendees = (from r in employeeAbsences
                                                  select new Select2Result(){ id = r.EmployeeId.ToString(), text = r.Name })
                                                 .ToList();


            Select2PagedResult result = new Select2PagedResult() { Results = pagedAttendees, Total = count };

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }



        //public ActionResult GetEmployeeAbsences(int month)
        //{
        //    var employeeAbsences = db.EmployeeAbsences
        //        .ToList()
        //        .Where(w => w.StartDate.Month == month || w.EndDate.Month == month)
        //        .Select(s => new { s.EmployeeAbsenceId, s.Employee.FullName, s.Employee.EmployeeCode, s.StartDate, s.EndDate, s.Comment });

        //    return Json(employeeAbsences, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult AddAbsence()
        {
            return PartialView("_CreateAbsence", new Absence());
        }

        public ActionResult SaveAbsence(Absence absence)
        {
            if (ModelState.IsValid)
            {
                db.Absences.Add(absence);
                db.SaveChanges();

                MyLogger.GetInstance.Info("Absence was created successfull, Name: " + absence.Name);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_CreateAbsence", absence);
        }

        // GET: EmployeeAbsences
        public ActionResult Index()
        {
            return View();
        }

        // GET: EmployeeAbsences/Create
        public ActionResult Create()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.EmployeeId = new SelectList(db.Employees.Where(w => companies.Contains(w.Department.CompanyId)), "EmployeeId", "CodeAndFullName");
            ViewBag.AbsenceId = new SelectList(db.Absences, "AbsenceId", "Name");

            var employeeAbsence = new EmployeeAbsence();
            employeeAbsence.StartDate = DateTime.Now;
            employeeAbsence.EndDate = DateTime.Now;

            return PartialView("_Create", employeeAbsence);
        }

        // POST: EmployeeAbsences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeAbsenceId,EmployeeId,AbsenceId,StartDate,EndDate,Comment")] EmployeeAbsence employeeAbsence)
        {
            if (ModelState.IsValid)
            {
                db.EmployeeAbsences.Add(employeeAbsence);
                db.SaveChanges();

                MyLogger.GetInstance.Info("Employee absence was created successfull, Employee: " + employeeAbsence.EmployeeId +" Absence: " + employeeAbsence.AbsenceId);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(w => companies.Contains(w.Department.CompanyId)), "EmployeeId", "CodeAndFullName");
            ViewBag.AbsenceId = new SelectList(db.Absences, "AbsenceId", "Name", employeeAbsence.AbsenceId);
            return PartialView("_Create", employeeAbsence);
        }

        // GET: EmployeeAbsences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeAbsence employeeAbsence = db.EmployeeAbsences.Find(id);
            if (employeeAbsence == null)
            {
                return HttpNotFound();
            }
            ViewBag.AbsenceId = new SelectList(db.Absences, "AbsenceId", "Name", employeeAbsence.AbsenceId);
            return PartialView("_Edit", employeeAbsence);
        }

        // POST: EmployeeAbsences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeAbsenceId,EmployeeId,AbsenceId,StartDate,EndDate,Comment")] EmployeeAbsence employeeAbsence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeAbsence).State = EntityState.Modified;
                db.SaveChanges();

                MyLogger.GetInstance.Info("Employee absence was edited successfull, Employee: " + employeeAbsence.EmployeeId + " Absence: " + employeeAbsence.AbsenceId);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.AbsenceId = new SelectList(db.Absences, "AbsenceId", "Name", employeeAbsence.AbsenceId);
            return PartialView("_Edit", employeeAbsence);
        }

        // GET: EmployeeAbsences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeAbsence employeeAbsence = db.EmployeeAbsences.Find(id);
            if (employeeAbsence == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", employeeAbsence);
        }

        // POST: EmployeeAbsences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeAbsence employeeAbsence = db.EmployeeAbsences.Find(id);
            db.EmployeeAbsences.Remove(employeeAbsence);
            db.SaveChanges();

            MyLogger.GetInstance.Info("Employee absence was deleted successfull, Employee: " + employeeAbsence.EmployeeId + " Absence: " + employeeAbsence.AbsenceId);

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

        //Extra classes to format the results the way the select2 dropdown wants them
        public class Select2PagedResult
        {
            public int Total { get; set; }
            public List<Select2Result> Results { get; set; }
        }

        public class Select2Result
        {
            public string id { get; set; }
            public string text { get; set; }
        }
    }
}
