using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.BLL;
using AttendanceRRHH.Models;
using Hangfire;
using System.Web.Security;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin")]
    public class ProcessController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Process
        public ActionResult Index()
        {
            ViewBag.company = new SelectList(db.Companies, "CompanyId", "Name");
            return View();
        }

        [AutomaticRetry(Attempts = 0)]
        public void Process(int companyId, DateTime date)
        {
            DailyProcess process = new DailyProcess(db);
            process.GenerateEmployeeTimeSheetByDayAndCompany(date, companyId);
        }


        [HttpPost]
        public JsonResult Run(string company, string date, bool? ReplaceRecords)
        {
            bool success = true;
            string message = Resources.Resources.Success;

            try
            {
                BackgroundJob.Enqueue(
                    () => Process(Int32.Parse(company), DateTime.Parse(date)));

                MyLogger.GetInstance.Info("Daily records was excuted for Company: " + company + " and date: " + date.ToString());
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}