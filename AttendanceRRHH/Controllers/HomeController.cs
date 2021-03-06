﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL;
using AttendanceRRHH.BLL;
using AttendanceRRHH.Helpers;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, Manager, User")]
    public class HomeController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            int totalActives = 0, totalInactives = 0, percent = 0;

            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();

            var employees = db.Employees.Include(i => i.Department).Where(w => companies.Contains(w.Department.CompanyId));

            if(employees != null){
                totalActives = employees.Where(w => w.IsActive).Count();
                totalInactives = employees.Where(w => w.IsActive == false).Count();
                percent = (totalActives / (totalActives + totalInactives)) * 100;
            }
        
            ViewBag.TotalActiveEmployees = totalActives;
            ViewBag.TotalInactiveEmployees = totalInactives;
            ViewBag.Percent = percent;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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