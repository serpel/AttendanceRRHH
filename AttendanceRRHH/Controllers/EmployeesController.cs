using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL.Security;
using AttendanceRRHH.BLL;

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
            var employees = db.Employees.Include(i => i.Department)
                .ToList()
                .Select(s => new { s.EmployeeId, s.EmployeeCode, Company = s.Department.Company.Name, s.NationalCardId, Name = s.FullName, s.PhoneNumber, Department = s.Department.Name, s.IsActive, s.IsExtraHourPay});

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name");
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.JobPositionId = new SelectList(db.Jobs, "JobPositionId", "JobTitle");
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name");
            return PartialView("_Create", new Employee());
        }

        //POST: Employees/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeId,EmployeeCode,NationalCardId,FirstName,LastName,Address,Bithdate,Gender,PhoneNumber,ProfileUrl,HireDate,DepartmentId,ShiftId,JobPositionId,CountryId,CityId,IsActive")] Employee employee)
        {
            string message = "";
            bool success = true;

            if (ModelState.IsValid)
            {
                try
                {
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

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs, "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", employee.ShiftId);
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

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs, "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", employee.ShiftId);

            return PartialView("_Edit", employee);
        }

        public ActionResult Edit2(int? id)
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

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs, "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", employee.ShiftId);

            return PartialView("_Edit", employee);
        }

        //POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeId,EmployeeCode,NationalCardId,FirstName,LastName,Address,Bithdate,Gender,PhoneNumber,ProfileUrl,HireDate,DepartmentId,ShiftId,JobPositionId,CountryId,CityId,IsActive")] Employee employee)
        {
            string message = "";
            bool success = false;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    success = true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                }

                return Json(new { success = success, message = message });
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", employee.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name", employee.CountryId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", employee.DepartmentId);
            ViewBag.JobPositionId = new SelectList(db.Jobs, "JobPositionId", "JobTitle", employee.JobPositionId);
            ViewBag.ShiftId = new SelectList(db.Shifts, "ShiftId", "Name", employee.ShiftId);
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
