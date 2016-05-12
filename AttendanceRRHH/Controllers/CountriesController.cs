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

namespace AttendanceRRHH.Controllers
{
    public class CountriesController : BaseController
    {
        private AttendanceContext db = new AttendanceContext();

        public ActionResult Cities(int? CountryId)
        {
            var cities = db.Cities
                .Where(w => w.CountryId == CountryId)
                .Take(10);

            return PartialView("_Cities", cities.ToList());
        }

        // GET: Countries
        public ActionResult Index()
        {
            return View();
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            return PartialView("_Create", new Country());
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CountryId,Name,IsActive")] Country country)
        {
            string message = "";

            if (ModelState.IsValid)
            {
                try
                {
                    db.Countries.Add(country);
                    db.SaveChanges();
                }catch(Exception e)
                {
                    message = e.Message;
                }

                return Json(new { success = true, url = Url.Action("Index", "Countries"), message = message });
            }

            return PartialView("_Create", country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CountryId,Name,IsActive")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return PartialView("_Edit", country);
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            try
            {
                Country country = db.Countries.Find(id);
                db.Countries.Remove(country);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new { success = true, message = message });
        }

        private CountryRepository cRepository = new CountryRepository();
        public JsonResult GetCountries()
        {
            var countries = cRepository.GetCountryList()
                .Select(c => new
                {
                    c.CountryId,
                    c.Name,
                    c.IsActive
                });

            return Json(countries, JsonRequestBehavior.AllowGet);
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
