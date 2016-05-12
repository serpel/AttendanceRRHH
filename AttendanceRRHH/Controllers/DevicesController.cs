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

namespace AttendanceRRHH.Controllers
{
    public class DevicesController : BaseController
    {
        private AttendanceContext db = new AttendanceContext();

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

        // GET: Devices/Details/5
        public ActionResult Details(int? id)
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
            return View(device);
        }

        // GET: Devices/Create
        public ActionResult Create()
        {
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name");
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeviceId,DeviceTypeId,Description,Location,IP,Port,IsSSR,OpenDoors")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Devices.Add(device);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name", device.DeviceTypeId);
            return View(device);
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
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeviceId,DeviceTypeId,Description,Location,IP,Port,IsSSR,OpenDoors")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "DeviceTypeId", "Name", device.DeviceTypeId);
            return View(device);
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
            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Refresh()
        {

            var deviseList = db.Devices
                .Where(w => w.IsActive == true)
                .Select(s => new ZKDevice()
                {
                    Id = s.DeviceId,
                    Description = s.Description,
                    Location = s.Location,
                    Ip = s.IP,
                    Port = s.Port,
                    IsSSR = s.IsSSR,
                    Status = Status.Unavailable,
                    Type = "ZK"
                });

            foreach (IDevice device in deviseList)
            {
                BackgroundJob.Enqueue(
                    () => getStatusbyDevise(device.Id));
            }

            return RedirectToAction("Index");
        }

        public void getStatusbyDevise(int id)
        {
            IDevice devise = db.Devices
                .Where(w => w.DeviceId == id)
                .Select(s => new ZKDevice()
                {
                    Id = s.DeviceId,
                    Description = s.Description,
                    Location = s.Location,
                    Ip = s.IP,
                    Port = s.Port,
                    IsSSR = s.IsSSR,
                    Status = Status.Unavailable,
                    Type = "ZK"
                }).Single();

            devise.GetStatus();
        }

        public void getStatus()
        {
            var deviseList = db.Devices
                .Where(w => w.IsActive == true)
                .Select(s => new ZKDevice()
                {
                    Id = s.DeviceId,
                    Description = s.Description,
                    Location = s.Location,
                    Ip = s.IP,
                    Port = s.Port,
                    IsSSR = s.IsSSR,
                    Status = Status.Unavailable,
                    Type = "ZK"
                });

            foreach (IDevice device in deviseList)
            {
                device.GetStatus();
            }
        }

        //public ActionResult Index()
        //{
        //    var deviseList = db.Devices
        //        .Where(w => w.IsActive == true)
        //        .Select(s => new ZKDevice()
        //        {
        //            Id = s.DeviceId,
        //            Description = s.Description,
        //            Location = s.Location,
        //            Ip = s.IP,
        //            Port = s.Port,
        //            IsSSR = s.IsSSR,
        //            Status = Status.Unavailable,
        //            Type = "ZK"
        //        });

        //    ViewBag.countAvailable = deviseList.Count(r => r.Status == Status.Available);
        //    ViewBag.countUnavailable = deviseList.Count(r => r.Status == Status.Unavailable);
        //    ViewBag.countUnknown = deviseList.Count(r => r.Status == Status.Unknown);

        //    return View(deviseList);
        //}
     

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


        public ActionResult SyncTime(int id)
        {
            DeviceFactory factory = new DeviceFactory();

            IDevice device = db.Devices
                .Include(d => d.DeviceType)
                .ToList()
                .Where(w => w.IsActive == true
                       && w.DeviceId == id)
                .Select(s => factory.CreateIntance(s.IP, (int)s.Port, s.DeviceType.Name, s.Description, s.Location, s.IsSSR))
                .FirstOrDefault();

            if (device == null)
                return Redirect("~/error404.html");

            if (device.SyncTime())
            {
                TempData["Message"] = "Successful to Sync Time";
            }
            else { TempData["Message"] = "Error at sync Time, contact to admin "; }

            return RedirectToAction("Index");
        }

        public bool TranferRecordsByDevise(int deviceId)
        {
            bool result = false;

            DeviceFactory factory = new DeviceFactory();

            IDevice device = db.Devices.Include(d => d.DeviceType)
                .Where(w => w.IsActive == true
                       && w.DeviceId == deviceId)
                .Select(s => factory.CreateIntance(s.IP, s.Port, s.DeviceType.Name, s.Description, s.Location, s.IsSSR))
                .Single();

            if (device != null)
            {
                result = device.TransferRecords();

                if (result)
                {
                    device.ClearDevice();
                }
            }

            return result;
        }

        public ActionResult TransferRecords(int id)
        {
            TempData["Message"] = "The records beign transfer";

            BackgroundJob.Enqueue(
                   () => TranferRecordsByDevise(id));

            return RedirectToAction("Index");
        }

        public ActionResult StartJob(string syncTimeExpression, string tranferRecordsExpression, string emailTimeExpression)
        {
            try
            {
                RecurringJob.AddOrUpdate(() => SyncTimeAllDevices(), syncTimeExpression, TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate(() => ReadRecordsAllDevices(), tranferRecordsExpression, TimeZoneInfo.Local);

                TempData["Message"] = "Task Created successful";
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error at create task: " + e.Message;
            }

            return RedirectToAction("Settings");
        }

        public void SyncTimeAllDevices()
        {
            DeviceFactory factory = new DeviceFactory();

            List<IDevice> deviceList = db.Devices.Include(d => d.DeviceType)
                .Where(w => w.IsActive == true)
                .Select(s => factory.CreateIntance(s.IP, s.Port, s.DeviceType.Name, s.Description, s.Location, s.IsSSR))
                .ToList();

            foreach (IDevice device in deviceList)
            {
                device.SyncTime();
            }
        }

        public void ReadRecordsAllDevices()
        {
            DeviceFactory factory = new DeviceFactory();

            List<IDevice> deviceList = db.Devices.Include(d => d.DeviceType)
                .Where(w => w.IsActive == true)
                .Select(s => factory.CreateIntance(s.IP, s.Port, s.DeviceType.Name, s.Description, s.Location, s.IsSSR))
                .ToList();

            foreach (IDevice device in deviceList)
            {
                if (device.TransferRecords())
                {
                    device.ClearDevice();
                }
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
