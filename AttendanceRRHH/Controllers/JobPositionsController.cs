using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager")]
    public class JobPositionsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult GetJobPositions()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var jobs = db.Jobs
                .Include(i => i.Company)
                .Where(w => companies.Contains((int)w.CompanyId))
                .ToList()
                .Select(s => new { s.JobPositionId, s.JobTitle, s.IsActive, Company = s.Company.Name });

            return Json(jobs, JsonRequestBehavior.AllowGet);
        }

        // GET: JobPositions
        public ActionResult Index()
        {
            return View();
        }

        // GET: JobPositions/Create
        public ActionResult Create()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name");

            return PartialView("_Create", new JobPosition());
        }

        // POST: JobPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JobPositionId,JobTitle,IsActive,CompanyId")] JobPosition jobPosition)
        {
            if (ModelState.IsValid)
            {
                db.Jobs.Add(jobPosition);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name", jobPosition.CompanyId);

            return PartialView("_Create", jobPosition);
        }

        // GET: JobPositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobPosition jobPosition = db.Jobs.Find(id);
            if (jobPosition == null)
            {
                return HttpNotFound();
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.CompanyId = new SelectList(db.Companies.Where(w => companies.Contains((int)w.CompanyId)), "CompanyId", "Name", jobPosition.CompanyId);

            return PartialView("_Edit", jobPosition);
        }

        // POST: JobPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JobPositionId,JobTitle,IsActive,CompanyId")] JobPosition jobPosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobPosition).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true}, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_Edit", jobPosition);
        }

        // GET: JobPositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobPosition jobPosition = db.Jobs.Find(id);
            if (jobPosition == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", jobPosition);
        }

        // POST: JobPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobPosition jobPosition = db.Jobs.Find(id);
            db.Jobs.Remove(jobPosition);
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
