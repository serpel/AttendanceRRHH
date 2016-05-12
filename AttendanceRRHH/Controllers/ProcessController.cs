﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.BLL;
using AttendanceRRHH.Models;
using Hangfire;

namespace AttendanceRRHH.Controllers
{
    public class ProcessController : BaseController
    {
        private AttendanceContext db = new AttendanceContext();
        // GET: Process
        public ActionResult Index()
        {
            ViewBag.company = new SelectList(db.Companies, "CompanyId", "Name");
            return View();
        }

        public void Process(int companyId, DateTime date)
        {
            DailyProcess process = new DailyProcess();
            process.GenerateEmployeeTimeSheetByDayAndCompany(date, companyId);
        }

        public JsonResult Run(string company, string date)
        {
            bool success = false;
            string message = "";

            try
            {
                BackgroundJob.Enqueue(
                    () => Process(Int32.Parse(company), DateTime.Parse(date)));

                success = true;
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new { success = success, message = message });
        }
    }
}