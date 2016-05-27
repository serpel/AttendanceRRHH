using System;
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
    public class ShiftTimesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShiftTimes
        public ActionResult Index()
        {
            var shiftTime = db.ShiftTimes.Include(s => s.Shift);
            return View(shiftTime.ToList());
        }

        // GET: ShiftTimes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftTime shiftTime = db.ShiftTimes.Find(id);
            if (shiftTime == null)
            {
                return HttpNotFound();
            }
            return View(shiftTime);
        }

        // GET: ShiftTimes/Create
        public ActionResult Create()
        {
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name");
            return View();
        }

        // POST: ShiftTimes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShiftTimeId,ShiftId,DayNumber,IsLaborDay,StartTime,EndTime,HasLunchTime,LunchStartTime,LunchEndTime,InsertedAt,UpdatedAt,IsActive")] ShiftTime shiftTime)
        {
            if (ModelState.IsValid)
            {
                db.ShiftTimes.Add(shiftTime);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", shiftTime.ShiftId);
            return View(shiftTime);
        }

        // GET: ShiftTimes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftTime shiftTime = db.ShiftTimes.Find(id);
            if (shiftTime == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", shiftTime.ShiftId);
            return View(shiftTime);
        }

        // POST: ShiftTimes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShiftTimeId,ShiftId,DayNumber,IsLaborDay,StartTime,EndTime,HasLunchTime,LunchStartTime,LunchEndTime,InsertedAt,UpdatedAt,IsActive")] ShiftTime shiftTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shiftTime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", shiftTime.ShiftId);
            return View(shiftTime);
        }

        // GET: ShiftTimes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftTime shiftTime = db.ShiftTimes.Find(id);
            if (shiftTime == null)
            {
                return HttpNotFound();
            }
            return View(shiftTime);
        }

        // POST: ShiftTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShiftTime shiftTime = db.ShiftTimes.Find(id);
            db.ShiftTimes.Remove(shiftTime);
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
