using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceRRHH.Models;
using AttendanceRRHH.BLL;
using Hangfire;
using AttendanceRRHH.DAL.Security;

namespace AttendanceRRHH.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin")]
    public class DevicesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public override ActionResult LeftNavBar()
        {
            ViewBag.DeviseUnavailableCount = db.Devices.Count(c => c.IsActive == true);
            return PartialView("_LeftNavBar");
        }

        // GET: Devices
        public ActionResult Index()
        {
            var devices = db.Devices.Include(d => d.DeviceType);

            ViewBag.countAvailable = 0;
            ViewBag.countUnavailable = 0;
            ViewBag.countUnknown = 0;

            return View(devices.ToList());
        }

        public ActionResult GetDevices()
        {
            var result = db.Devices
                .ToList()
                .Select(s => new { s.DeviceId, s.IP, s.Description, s.IsActive, s.IsSSR, s.Location, s.Port, s.OpenDoors, Type = s.DeviceType.Name,
                    SyncDate = s.SyncDate.ToString("yyyy/MM/dd hh:mm"),
                    DeviceStatus = s.DeviceStatus == DeviceStatus.Available ? Resources.Resources.Available
                                    : s.DeviceStatus == DeviceStatus.Unavailable ? Resources.Resources.Unavailable
                                    : s.DeviceStatus == DeviceStatus.Unknown ? Resources.Resources.Unknown : "" });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: Devices/Create
        public ActionResult Create()
        {
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name");

            var device = new Device();
            return PartialView("Create", device);
        }

        // POST: Devices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeviceId,DeviceTypeId,Description,Location,IP,Port,IsSSR,IsActive,OpenDoors,SyncTimeCronExpression,TransferCronExpression,SyncDate")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Devices.Add(device);
                db.SaveChanges();

                RecurringJob.AddOrUpdate("s" + device.DeviceId.ToString(), () => SyncTimeAndTransferByDevice(device.DeviceId), device.SyncTimeCronExpression, TimeZoneInfo.Local);
                //RecurringJob.AddOrUpdate("t" + device.DeviceId.ToString(), () => TransferRecordsByDevice(device.DeviceId), device.TransferCronExpression);

                MyLogger.GetInstance.Info("Device was created succesfull, Id: " + device.DeviceId + ", Name: " + device.Description);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name", device.DeviceTypeId);
            return PartialView("Device", device);
        }

        // GET: Devices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name", device.DeviceTypeId);
            return PartialView("Edit", device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeviceId,DeviceTypeId,Description,Location,IP,Port,IsSSR,IsActive,OpenDoors")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();

                RecurringJob.AddOrUpdate("s" + device.DeviceId.ToString(), () => SyncTimeAndTransferByDevice(device.DeviceId), device.SyncTimeCronExpression, TimeZoneInfo.Local);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name", device.DeviceTypeId);
            return PartialView("Edit", device);
        }

        // GET: Devices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();

            RecurringJob.RemoveIfExists("s" + device.DeviceId);
            //RecurringJob.RemoveIfExists("t" + device.DeviceId);

            MyLogger.GetInstance.Info("Device was delete succesfull, Id: " + id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveDevice(int id)
        {
            try
            {
                var device = db.Devices.Find(id);
                db.Devices.Remove(device);
                db.SaveChanges();

                TempData["Message"] = "Device id " + id + " was removed successful.";
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error on remove device, " + e.Message;
            }
            return RedirectToAction("Index");
        }


        public ActionResult PingAllDevices()
        {
            TempData["Message"] = Resources.Resources.PingTestAll;

            var devices = db.Devices.Where(w => w.IsActive == true).ToList();

            foreach(var device in devices)
            {
                BackgroundJob.Enqueue(
                        () => GetStatus(device.DeviceId));              
            }

            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }


        public ActionResult Ping(int id)
        {             
            BackgroundJob.Enqueue(
                        () => GetStatus(id));

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [AutomaticRetry(Attempts = 0)]
        public void GetStatus(int id)
        {
            DeviceFactory factory = new DeviceFactory();

            var device = db.Devices.Include(i => i.DeviceType).Where(w => w.DeviceId == id).FirstOrDefault();
            if (device != null)
            {
                IDevice iDevice = factory.CreateIntance(device);
                iDevice.GetStatus();
            }
        }


        public ActionResult SyncTime(int id)
        {
            TempData["Message"] = Resources.Resources.Execution;

            BackgroundJob.Enqueue(
                        () => SyncTimeByDevice(id));

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [AutomaticRetry(Attempts = 0)]
        public bool TranferRecordsByDevise(int deviceId)
        {
            bool result = false;

            var device = db.Devices.Include(d => d.DeviceType).Where(w => w.DeviceId == deviceId).FirstOrDefault();

            DeviceFactory factory = new DeviceFactory();

            IDevice iDevice = factory.CreateIntance(device);

            if (iDevice.TransferRecords())
            {
                if (iDevice.ClearDevice())
                    result = true;
            }

            return result;
        }

        public ActionResult TransferRecords(int id)
        {
            TempData["Message"] = "The records beign transfer";

            BackgroundJob.Enqueue(
                   () => TranferRecordsByDevise(id));

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SyncTimeAllDevices()
        {
            TempData["Message"] = Resources.Resources.SyncTimeAllDevicesText;

            var deviceList = db.Devices.Where(w => w.IsActive == true).ToList();

            foreach (var device in deviceList)
            {
                BackgroundJob.Enqueue(
                       () => SyncTimeByDevice(device.DeviceId));
            }

            return RedirectToAction("Index");

            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [AutomaticRetry(Attempts = 0)]
        public void SyncTimeAndTransferByDevice(int deviceId)
        {
            MyLogger.GetInstance.Info("SyncTimeAndTransferByDevice");

            var device = db.Devices.Include(d => d.DeviceType).Where(w => w.DeviceId == deviceId).FirstOrDefault();

            DeviceFactory factory = new DeviceFactory();

            IDevice iDevice = factory.CreateIntance(device);

            if(iDevice.SyncTime())
            {
                if(iDevice.TransferRecords())
                    iDevice.ClearDevice();
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void SyncTimeByDevice(int deviceId)
        {
            var device = db.Devices.Include(d => d.DeviceType).Where(w => w.DeviceId == deviceId).FirstOrDefault();

            DeviceFactory factory = new DeviceFactory();

            IDevice iDevice = factory.CreateIntance(device);
            
            iDevice.SyncTime();
        }

        public ActionResult ReadRecordsAllDevices()
        {
            TempData["Message"] = Resources.Resources.TransferAll;

            var deviceList = db.Devices.Where(w => w.IsActive == true).ToList();

            foreach (var device in deviceList)
            {
                BackgroundJob.Enqueue(
                       () => TransferRecordsByDevice(device.DeviceId));
            }

            return RedirectToAction("Index");
        }

        [AutomaticRetry(Attempts = 0)]
        public void TransferRecordsByDevice(int deviceId)
        {
            var device = db.Devices.Include(d => d.DeviceType).Where(w => w.DeviceId == deviceId).FirstOrDefault();

            DeviceFactory factory = new DeviceFactory();

            IDevice iDevice = factory.CreateIntance(device);

            if (iDevice.TransferRecords())
            {
                iDevice.ClearDevice();
            }
        }

        public ActionResult Settings()
        {
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
