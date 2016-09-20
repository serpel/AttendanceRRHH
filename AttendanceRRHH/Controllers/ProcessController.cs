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
    [AccessAuthorizeAttribute(Roles = "Admin,Manager")]
    public class ProcessController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Process
        public ActionResult Index()
        {
            var companies = db.UserCompanies.Where(w => w.User.UserName == User.Identity.Name).Select(s => s.CompanyId).Distinct().ToList();
            ViewBag.company = new SelectList(db.Companies.Where(w => companies.Contains(w.CompanyId)), "CompanyId", "Name");
            return View();
        }

        [AutomaticRetry(Attempts = 0)]
        public void Process(int companyId, DateTime date, bool replace)
        {
            DailyProcess process = new DailyProcess();

            if (replace)
            {
                process.GenerateEmployeeTimeSheetByDayAndCompany(date, companyId);
            }
            else
            {
                process.GenerateEmployeeTimeSheetByDayAndCompanyNonReplaceHours(date, companyId);
            }
        }


        [HttpPost]
        public JsonResult Run(string company, string date, bool? ReplaceRecords)
        {
            bool success = true;
            string message = Resources.Resources.Success;

            try
            {
                BackgroundJob.Enqueue(
                    () => Process(Int32.Parse(company), DateTime.Parse(date), (bool)ReplaceRecords));

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