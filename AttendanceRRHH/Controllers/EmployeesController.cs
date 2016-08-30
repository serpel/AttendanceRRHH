using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL.Security;
using AttendanceRRHH.BLL;
using System.Web;
using System.IO;
using System.Configuration;
using AttendanceRRHH.DAL;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager")]
    public class EmployeesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Settings()
        {
            return View();
        }

        // GET: Employees
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEmployees()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var employees = db.Employees.Include(i => i.Department)
                .Where(w => companies.Contains(w.Department.CompanyId))
                .ToList()
                .Select(s => new { s.EmployeeId, s.EmployeeCode, Company = s.Department.Company.Name, s.NationalCardId, Name = s.FullName, Department = s.Department.Name, s.IsActive, s.IsExtraHourPay, s.ProfileUrl });

            return Json(employees, JsonRequestBehavior.AllowGet);

        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name");
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(w => companies.Contains(w.CompanyId)).Select(s => new { s.DepartmentId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "DepartmentId", "Name");
            ViewBag.JobPositionId = new SelectList(db.Jobs.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.JobPositionId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.JobTitle }), "JobPositionId", "JobTitle");
            ViewBag.ShiftId = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.ShiftId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "ShiftId", "Name");
            return PartialView("_Create", new Employee());
        }

        //POST: Employees/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeId,EmployeeCode,NationalCardId,FirstName,LastName,Address,Bithdate,Gender,PhoneNumber,ProfileUrl,HireDate,DepartmentId,ShiftId,JobPositionId,CountryId,CityId,IsActive,IsExtraHourPay")] Employee employee, HttpPostedFileBase file)
        {
            string message = "";
            bool success = true;

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string baseUrl = Url.Content(Path.Combine(@ConfigurationManager.AppSettings["ProfileImagePath"], employee.EmployeeId.ToString()));
                        string path = Uploader.GetInstance.GenerateUrlPath(Server.MapPath(baseUrl), baseUrl, file);
                        employee.ProfileUrl = Url.Content(path);
                    }

                    db.Employees.Add(employee);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    message = e.Message;
                    success = false;
                }

                MyLogger.GetInstance.Info("Employee was created, Name: " + employee.FullName + ", Code: " + employee.EmployeeCode);

                return Json(new { success = success, message = message });
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(w => companies.Contains(w.CompanyId)).Select(s => new { s.DepartmentId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.JobPositionId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.JobTitle }), "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.ShiftId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "ShiftId", "Name", employee.ShiftId);
            return PartialView("_Create", employee);
        }

        //GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(w => companies.Contains(w.CompanyId)).Select(s => new { s.DepartmentId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.JobPositionId, JobTitle = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.JobTitle }), "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.ShiftId, Name = s.Company.Name.Substring(0,3).ToUpper() + " - " + s.Name}), "ShiftId", "Name", employee.ShiftId);

            return PartialView("_Edit", employee);
        }

        //POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeId,EmployeeCode,NationalCardId,FirstName,LastName,Address,Bithdate,Gender,PhoneNumber,ProfileUrl,HireDate,DepartmentId,ShiftId,JobPositionId,CountryId,CityId,IsActive,IsExtraHourPay")] Employee employee, HttpPostedFileBase file)
        {
            string message = "";
            bool success = false;

            if (ModelState.IsValid)
            {
                try
                {                 
                    if(file != null && file.ContentLength > 0)
                    {
                        string baseUrl = Url.Content(Path.Combine(@ConfigurationManager.AppSettings["ProfileImagePath"], employee.EmployeeId.ToString()));
                        string path = Uploader.GetInstance.GenerateUrlPath(Server.MapPath(baseUrl), baseUrl, file);
                        employee.ProfileUrl = Url.Content(path);
                    }

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();

                    success = true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    success = false;
                }

                return Json(new { success = success, message = message });
            }

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(w => companies.Contains(w.CompanyId)).Select(s => new { s.DepartmentId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.JobPositionId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.JobTitle }), "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts.Where(w => companies.Contains((int)w.CompanyId)).Select(s => new { s.ShiftId, Name = s.Company.Name.Substring(0, 3).ToUpper() + " - " + s.Name }), "ShiftId", "Name", employee.ShiftId);
            return PartialView("_Edit", employee);
        }

        //GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);

            if (employee == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            bool success = true;
            try
            {
                Employee employee = db.Employees.Find(id);
                db.Employees.Remove(employee);
                db.SaveChanges();

                MyLogger.GetInstance.Info("Employee was deleted, Name: " + employee.FullName + ", Code: " + employee.EmployeeCode);

            }
            catch (Exception e)
            {
                message = e.Message;
                success = false;
            }

            return Json(new { success = success, message = message });
        }
    }
}
