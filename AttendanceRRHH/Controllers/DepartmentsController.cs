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

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager")]
    public class DepartmentsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult GetDepartments()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var departments = db.Departments
                .Where(w => companies.Contains((int)w.CompanyId))
                .ToList()
                .Select(s => new { s.DepartmentId, Company = s.Company.Name, s.Name, s.IsActive });

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        // GET: Departments
        public ActionResult Index()
        {
            return View();
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");
            return PartialView("_Create", new Department());
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,CompanyId,Name,IsActive")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");

            return PartialView("_Create", department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name", department.CompanyId);
            return PartialView("_Edit", department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,CompanyId,Name,IsActive")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name", department.CompanyId);
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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
