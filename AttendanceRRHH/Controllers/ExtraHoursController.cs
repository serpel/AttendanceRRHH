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
    public class ExtraHoursController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ExtraHours
        public ActionResult Index()
        {
            return View(db.ExtraHours.ToList());
        }


        public ActionResult GetExtraHourDetails(int id)
        {
            var result = db.ExtraHourDetails.Where(w => w.ExtraHourId == id);

            return PartialView(result.ToList());
        }

        // GET: ExtraHours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHour extraHour = db.ExtraHours.Find(id);
            if (extraHour == null)
            {
                return HttpNotFound();
            }
            return View(extraHour);
        }

        // GET: ExtraHours/Create
        public ActionResult Create()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");

            return View();
        }

        // POST: ExtraHours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExtraHourId,Name,CompanyId")] ExtraHour extraHour)
        {
            if (ModelState.IsValid)
            {
                db.ExtraHours.Add(extraHour);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(extraHour);
        }

        // GET: ExtraHours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHour extraHour = db.ExtraHours.Find(id);
            if (extraHour == null)
            {
                return HttpNotFound();
            }
            return View(extraHour);
        }

        // POST: ExtraHours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExtraHourId,Name,CompanyId")] ExtraHour extraHour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extraHour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(extraHour);
        }

        // GET: ExtraHours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHour extraHour = db.ExtraHours.Find(id);
            if (extraHour == null)
            {
                return HttpNotFound();
            }
            return View(extraHour);
        }

        // POST: ExtraHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExtraHour extraHour = db.ExtraHours.Find(id);
            db.ExtraHours.Remove(extraHour);
            db.SaveChanges();
            return RedirectToAction("Index");
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
