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
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            return View(db.ExtraHours.Where(w => companies.Contains((int)w.CompanyId)).ToList());
        }

        public ActionResult EditExtraHourDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHourDetail extraHour = db.ExtraHourDetails.Find(id);
            if (extraHour == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditExtraHourDetails", extraHour);
        }

        [HttpPost]
        public ActionResult EditExtraHourDetails(ExtraHourDetail extrahourdetail)
        {
            string message = "";

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(extrahourdetail).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    message = e.Message;
                }
            }
            else
            {
                return PartialView("_EditExtraHourDetails", extrahourdetail);
            }

            return RedirectToAction("Edit", new { id = extrahourdetail.ExtraHourId });

            //return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateExtraHourDetails(int extra)
        {
            return PartialView("_CreateExtraHourDetails", new ExtraHourDetail() { ExtraHourId = extra });
        }

        [HttpPost]
        public ActionResult CreateExtraHourDetails(ExtraHourDetail extrahourdetail)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ExtraHourDetails.Add(extrahourdetail);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.Message;
                }
            }
            else
            {
                return PartialView("_CreateExtraHourDetails", extrahourdetail);
            }

            return RedirectToAction("Edit", new { id = extrahourdetail.ExtraHourId });
        }

        public ActionResult DeleteExtraHourDetails(int? id)
        {
            var result = db.ExtraHourDetails.Find(id);
            return PartialView("_DeleteExtraHourDetails", result);
        }

        [HttpPost]
        public ActionResult DeleteExtraHourDetails(ExtraHourDetail extrahourdetail)
        {
            try
            {
                var extra = db.ExtraHourDetails.Find(extrahourdetail.ExtraHourDetailId);     
                db.ExtraHourDetails.Remove(extra);
                db.SaveChanges();

            }catch(Exception e)
            {
                ViewBag.Message = e.Message;
            }

            return RedirectToAction("Edit", new { id = extrahourdetail.ExtraHourId });
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

            return View(new ExtraHourViewModel());
        }

        //// POST: ExtraHours/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ExtraHourId,Name,CompanyId")] ExtraHour extraHour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ExtraHours.Add(extraHour);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(extraHour);
        //}

        public JsonResult CreateExtraHourDetail(ExtraHourViewModel obj)
        {
            bool success = true;
            string message = Resources.Resources.Success;

            try{
                if (ModelState.IsValid)
                {
                    var extra = new ExtraHour()
                    {
                        CompanyId = obj.CompanyId,
                        Name = obj.Name                       
                    };
                    db.ExtraHours.Add(extra);
                    db.SaveChanges();

                    foreach(var extradetail in obj.ExtraDetails)
                    {
                        extradetail.ExtraHourId = extra.ExtraHourId;
                    }

                    db.ExtraHourDetails.AddRange(obj.ExtraDetails);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                success = false;
            }

            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        // GET: ExtraHours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtraHour extraHour = db.ExtraHours.Find(id);

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");

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
