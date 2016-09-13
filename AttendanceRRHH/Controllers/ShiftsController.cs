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
            //var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            //shift.ShiftList = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)), "ShiftId", "Name");

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
                    shiftedit.IsActive = shift.IsActive;
                    shiftedit.ExtraHourId = shift.ExtraHourId;

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

                    MyLogger.GetInstance.Info("Shift was edited successfull, Id: " + shift.ShiftId);

                    success = true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    success = false;
                    MyLogger.GetInstance.Error("Error", e);
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

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var shifts = db.Shifts.Include(i => i.Company).Where(w => companies.Contains((int)w.CompanyId));

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
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.ShiftId = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.ShiftId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - "+ s.Name }), "ShiftId", "Name");
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");
            ViewBag.ExtraHourId = new SelectList(db.ExtraHours.Where(w => companies.Contains((int)w.CompanyId)), "ExtraHourId", "Name");

            return View(new ShiftViewModel());
        }

        [HttpPost]
        public JsonResult Create(ShiftViewModel obj)
        {
            bool success = true;
            string message = "";

            if (ModelState.IsValid)
            {
                Shift s = new Shift()
                {
                    CompanyId = obj.CompanyId,
                    Name = obj.ShiftName,
                    Description = obj.ShiftDescription,
                    ExtraHourId =  obj.ExtraHourId,                   
                };

                try
                {
                    db.Shifts.Add(s);
                    db.SaveChanges();

                    foreach (ShiftTime t in obj.TimeList)
                    {
                        t.ShiftId = s.ShiftId;
                    }

                    db.ShiftTimes.AddRange(obj.TimeList);
                    db.SaveChanges();

                    MyLogger.GetInstance.Info("Shift was edited successfull, Id: " + obj.ShiftId);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }
            }

            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }


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

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.ExtraHourId = new SelectList(db.ExtraHours.Where(w => w.CompanyId == shift.CompanyId), "ExtraHourId", "Name", shift.ExtraHourId);

            return View(new ShiftViewModel()
            {
                ShiftId = shift.ShiftId, 
                ShiftName = shift.Name, 
                ShiftDescription = shift.Description,
                IsActive =  shift.IsActive,
                TimeList = shift.ShiftTimes.ToList()
            });     
        }

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

            MyLogger.GetInstance.Info("Shift was deleted successfull, Id: " + id);

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
