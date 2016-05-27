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
using PagedList;
using AttendanceRRHH.BLL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager")]
    public class ShiftsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private GenericRepository<Shift> sRepository = new GenericRepository<Shift>();

        public ActionResult ShiftTimeList()
        {

            ShiftViewModel shift = new ShiftViewModel()
            {
                TimeList = (List<ShiftTime>)Utils.Instance.CreateShiftTime(-1, 7)

            };
            return PartialView("_ShiftTimeList", shift);
        }

        public ActionResult ShiftTimeListEdit(ShiftViewModel shift)
        {
            return PartialView("_ShiftTimeList", shift);
        }

        [HttpPost]
        public ActionResult EditShift(ShiftViewModel shift)
        {
            bool success = false;
            string message = "";
              
            if (ModelState.IsValid)
            {
                var shiftedit = db.Shifts
                    .Where(w => w.ShiftId == shift.ShiftId).FirstOrDefault();

                if (shiftedit != null)
                {
                    shiftedit.Name = shift.ShiftName;
                    shiftedit.Description = shift.ShiftDescription;
                    shiftedit.UpdatedAt = DateTime.Now;
                    db.Entry(shiftedit).State = EntityState.Modified;
                }

                foreach (var t in shift.TimeList)
                {
                    ShiftTime shifttime = db.ShiftTimes.Find(t.ShiftTimeId);

                    if (shifttime != null)
                    {
                        shifttime.StartTime = t.StartTime;
                        shifttime.EndTime = t.EndTime;
                        shifttime.HasLunchTime = t.HasLunchTime;
                        shifttime.IsLaborDay = t.IsLaborDay;
                        shifttime.IsActive = t.IsActive;
                        shifttime.LunchEndTime = t.LunchEndTime;
                        shifttime.LunchStartTime = t.LunchStartTime;
                        shifttime.UpdatedAt = DateTime.Now;
                        db.Entry(shifttime).State = EntityState.Modified;
                    }
                }

                try {
                    db.SaveChanges();
                    success = true;
                } catch(Exception e)
                {
                    message = e.Message;
                }
            }
            return Json(new { success = success, message = message });
        }

        [HttpPost]
        public JsonResult AddShift(ShiftViewModel obj)
        {
            bool success = false;
            string message = "";

            if (ModelState.IsValid)
            {
                success = true;
                Shift s = new Shift()
                {
                    CompanyId = obj.CompanyId,
                    Name = obj.ShiftName,
                    Description = obj.ShiftDescription,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };

                try
                {
                    db.Shifts.Add(s);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    message = e.Message;
                }

                foreach (ShiftTime t in obj.TimeList)
                {
                    t.ShiftId = s.ShiftId;
                    t.InsertedAt = DateTime.Now;
                    t.UpdatedAt = DateTime.Now;
                    t.IsActive = true;
                    t.IsLaborDay = true;
                }

                try
                {
                    db.ShiftTimes.AddRange(obj.TimeList);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }

            return Json(new { success = success, message = message });
        }


        // GET: Shifts
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var shifts = sRepository.GetDbSet().Include(s => s.Company);

            if (!String.IsNullOrEmpty(searchString))
            {
                shifts = shifts.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    shifts = shifts.OrderByDescending(s => s.Name);
                    break;
                case "date_desc":
                    shifts = shifts.OrderByDescending(s => s.Description);
                    break;
                default:
                    shifts = shifts.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(shifts.ToPagedList(pageNumber, pageSize));
        }

        // GET: Shifts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = db.Shifts.Find(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // GET: Shifts/Create
        public ActionResult Create()
        {
            ViewBag.ShiftId = new SelectList(sRepository.GetAll(), "ShiftId", "Name");
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name");

            return View();
        }

        //POST: Shifts/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(ShiftViewModel shift)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Shift s = new Shift()
        //        {
        //            CompanyId = shift.CompanyId,
        //            Name = shift.ShiftName,
        //            Description = shift.ShiftDescription,
        //            InsertedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now,
        //            IsActive = true                    
        //        };

        //        db.Shifts.Add(s);
        //        db.SaveChanges();

        //        Shift f = db.Shifts.Find(s.ShiftId);
        //        foreach(ShiftTime t in f.ShiftTimes)
        //        {
        //            t.ShiftId = f.ShiftId;
        //        }
        //        db.ShiftTime.AddRange(s.ShiftTimes);
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", shift.CompanyId);
        //    return View(shift);
        //}

        // GET: Shifts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = db.Shifts.Where(w => w.ShiftId == id).FirstOrDefault();
            if (shift == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", shift.CompanyId);
            return View(new ShiftViewModel()
            {
                ShiftId = shift.ShiftId, 
                ShiftName = shift.Name, 
                ShiftDescription = shift.Description,
                TimeList = shift.ShiftTimes.ToList()
            });
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ShiftId,CompanyId,Name,Description,IsSpecialShift,InsertedAt,UpdatedAt,IsActive")] Shift shift)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(shift).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", shift.CompanyId);
        //    return View(shift);
        //}

        // GET: Shifts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shift shift = db.Shifts.Find(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shift shift = db.Shifts.Find(id);
            db.Shifts.Remove(shift);
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
