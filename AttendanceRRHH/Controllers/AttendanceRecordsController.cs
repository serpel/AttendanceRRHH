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
    public class AttendanceRecordsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AttendanceRecords
        public ActionResult Index()
        {
            var attendanceRecords = db.AttendanceRecords.Include(a => a.Device).Include(a => a.Employee);
            return View(attendanceRecords.ToList());
        }

        // GET: AttendanceRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceRecord attendanceRecord = db.AttendanceRecords.Find(id);
            if (attendanceRecord == null)
            {
                return HttpNotFound();
            }
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Create
        public ActionResult Create()
        {
            ViewBag.DeviceId = new SelectList(db.Devices, "DeviceId", "Description");
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode");
            return View();
        }

        // POST: AttendanceRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AttendanceRecodId,DeviceId,EmployeeId,Date,InsertedAt")] AttendanceRecord attendanceRecord)
        {
            if (ModelState.IsValid)
            {
                attendanceRecord.InsertedAt = DateTime.Now;
                db.AttendanceRecords.Add(attendanceRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceId = new SelectList(db.Devices, "DeviceId", "Description", attendanceRecord.DeviceId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode", attendanceRecord.EmployeeId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceRecord attendanceRecord = db.AttendanceRecords.Find(id);
            if (attendanceRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceId = new SelectList(db.Devices, "DeviceId", "Description", attendanceRecord.DeviceId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode", attendanceRecord.EmployeeId);
            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AttendanceRecodId,DeviceId,EmployeeId,Date,InsertedAt")] AttendanceRecord attendanceRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendanceRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceId = new SelectList(db.Devices, "DeviceId", "Description", attendanceRecord.DeviceId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeCode", attendanceRecord.EmployeeId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceRecord attendanceRecord = db.AttendanceRecords.Find(id);
            if (attendanceRecord == null)
            {
                return HttpNotFound();
            }
            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttendanceRecord attendanceRecord = db.AttendanceRecords.Find(id);
            db.AttendanceRecords.Remove(attendanceRecord);
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
