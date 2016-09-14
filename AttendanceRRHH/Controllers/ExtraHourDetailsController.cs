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
    public class ExtraHourDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ExtraHourDetails
        public ActionResult Index()
        {
            var extraHourDetails = db.ExtraHourDetails.Include(e => e.ExtraHour);
            return View(extraHourDetails.ToList());
        }

        // GET: ExtraHourDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHourDetail extraHourDetail = db.ExtraHourDetails.Find(id);
            if (extraHourDetail == null)
            {
                return HttpNotFound();
            }
            return View(extraHourDetail);
        }

        // GET: ExtraHourDetails/Create
        public ActionResult Create()
        {
            ViewBag.ExtraHourId = new SelectList(db.ExtraHours, "ExtraHourId", "Name");
            return View();
        }

        // POST: ExtraHourDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExtraHourDetailId,ExtraHourId,Code,StartHour,EndHour,Day")] ExtraHourDetail extraHourDetail)
        {
            if (ModelState.IsValid)
            {
                db.ExtraHourDetails.Add(extraHourDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ExtraHourId = new SelectList(db.ExtraHours, "ExtraHourId", "Name", extraHourDetail.ExtraHourId);
            return View(extraHourDetail);
        }

        // GET: ExtraHourDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHourDetail extraHourDetail = db.ExtraHourDetails.Find(id);
            if (extraHourDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExtraHourId = new SelectList(db.ExtraHours, "ExtraHourId", "Name", extraHourDetail.ExtraHourId);
            return View(extraHourDetail);
        }

        // POST: ExtraHourDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExtraHourDetailId,ExtraHourId,Code,StartHour,EndHour,Day")] ExtraHourDetail extraHourDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(extraHourDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExtraHourId = new SelectList(db.ExtraHours, "ExtraHourId", "Name", extraHourDetail.ExtraHourId);
            return View(extraHourDetail);
        }

        // GET: ExtraHourDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHourDetail extraHourDetail = db.ExtraHourDetails.Find(id);
            if (extraHourDetail == null)
            {
                return HttpNotFound();
            }
            return View(extraHourDetail);
        }

        // POST: ExtraHourDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExtraHourDetail extraHourDetail = db.ExtraHourDetails.Find(id);
            db.ExtraHourDetails.Remove(extraHourDetail);
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
