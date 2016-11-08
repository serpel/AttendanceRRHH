using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttendanceRRHH.Models;
using NLog;
using System.Data.Entity;

namespace AttendanceRRHH.BLL
{
    public class ZKDevice : IDevice, IDisposable
    {
        private Device _device;

        private ApplicationDbContext context;

        public Device Device
        {
            get
            {
                return this._device;
            }

            set
            {
                this._device = value;

            }
        }

        #region ZKLibrary definitions
        private zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();
        private bool bIsConnected = false;
        private int iMachineNumber = 1;
        #endregion

        public ZKDevice()
        {
            context = new ApplicationDbContext();
        }

        public ZKDevice(ApplicationDbContext context)
        {
            this.context = context;
        }

        public ZKDevice(int device, ApplicationDbContext context)
        {
            this.Device = context.Devices.Find(device);
            this.context = context;
        }

        private void ConnectDevice()
        {
            if (bIsConnected)
                return;

            int idwErrorCode = 0;

            try
            {
                bIsConnected = axCZKEM1.Connect_Net(this._device.IP, this._device.Port);
                if (bIsConnected)
                {
                    //this.Device.DeviceStatus = DeviceStatus.Available;
                    iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    MyLogger.GetInstance.Info("ZKDevice.ConnectDevice() - The device is connected successull " + this.Device.IP);
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    MyLogger.GetInstance.Error(string.Format("ZKDevice.ConnectDevice() - Unable to connect the Device: {0}, ErrorCode: {1}", this.Device.IP, idwErrorCode.ToString()));
                }
            }
            catch (Exception e)
            {
                MyLogger.GetInstance.Error(e.Message);
            }
        }

        private void DisconnectDevice()
        {
            this.axCZKEM1.Disconnect();
            this.bIsConnected = false;
            MyLogger.GetInstance.Info("ZKDevice.DisconnectDevice() - Device Disconnected: " + this.Device.IP);
        }

        private bool CompareDates(DateTime date1, DateTime date2)
        {
            if (date1.Year == date2.Year &&
                date1.Month == date2.Month &&
                date1.Day == date2.Day)
            {
                return true;
            }
            return false;
        }

        private bool SSRDownloadLogData()
        {
            bool result = true;
            String idwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idWorkCode = 0;
            int idwErrorCode = 0;
            int iGLCount = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                var device = context.Devices
                            .Where(w => w.IP == this.Device.IP).FirstOrDefault();

                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out idwEnrollNumber,
                       out idwVerifyMode, out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idWorkCode))//get records from the memory
                {

                    iGLCount++;
                    var dtStr = idwYear.ToString() + '-' + idwMonth.ToString() + '-' + idwDay.ToString() + ' ' + idwHour.ToString() + ':' + idwMinute.ToString() + ':' + idwSecond.ToString();
                    var dt = DateTime.Parse(dtStr);

                    if (dt != null)
                    {
                        var employee = context.Employees
                                       .Where(w => w.EmployeeCode == idwEnrollNumber)
                                       .FirstOrDefault();

                        if (employee != null)
                        {
                            AttendanceRecord record = new AttendanceRecord()
                            {
                                EmployeeId = employee.EmployeeId,
                                Date = dt,
                                DeviceId = device.DeviceId,
                                InsertedAt = DateTime.Now
                            };

                            try
                            {
                                context.AttendanceRecords.Add(record);
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                result = false;
                                MyLogger.GetInstance.Error(string.Format("ZKDevice.SSRDownloadLogData() - Error on device Ip: {0} ", this.Device.IP), e);
                            }

                            MyLogger.GetInstance.Info(string.Format("ZKDevice.SSRDownloadLogData() - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dt.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                        }
                        else
                        {
                            MyLogger.GetInstance.Error(string.Format("ZKDevice.SSRDownloadLogData() - Employee Not Exist - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dt.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                        }
                    }
                    else
                    {
                        result = false;
                        MyLogger.GetInstance.Error(string.Format("ZKDevice.SSRDownloadLogData() - Time Error - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dtStr, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                    }
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                result = false;

                if (idwErrorCode != 0)
                {
                    MyLogger.GetInstance.Error("ZKDevice.SSRDownloadLogData() - Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    MyLogger.GetInstance.Error("ZKDevice.SSRDownloadLogData() - No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true); //enable the device
            MyLogger.GetInstance.Debug("ZKDevice.SSRDownloadLogData() - Download: " + iGLCount + " records");

            return result;
        }

        private bool DownloadLogData()
        {
            bool result = true;
            int idwTMachineNumber = 0;
            int idwEnrollNumber = 0;
            int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;

            int idwErrorCode = 0;
            int iGLCount = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                var device = context.Devices
                     .Where(w => w.IP == this.Device.IP).FirstOrDefault();

                while (axCZKEM1.GetGeneralLogData(iMachineNumber, ref idwTMachineNumber, ref idwEnrollNumber,
                        ref idwEMachineNumber, ref idwVerifyMode, ref idwInOutMode, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute))//get records from the memory
                {

                        iGLCount++;
                        var dtStr = idwYear.ToString() + '-' + idwMonth.ToString() + '-' + idwDay.ToString() + ' ' + idwHour.ToString() + ':' + idwMinute.ToString();
                        var dt = DateTime.Parse(dtStr);

                        if (dt != null)
                        {
                            var employee = context.Employees
                                           .Where(w => w.EmployeeCode == idwEnrollNumber.ToString())
                                           .FirstOrDefault();

                            if (employee != null)
                            {
                                AttendanceRecord record = new AttendanceRecord()
                                {
                                    EmployeeId = employee.EmployeeId,
                                    Date = dt,
                                    DeviceId = device.DeviceId,
                                    InsertedAt = DateTime.Now
                                };
                           
                            try
                            {
                                context.AttendanceRecords.Add(record);
                                context.SaveChanges();                               
                            }
                            catch (Exception e)
                            {
                                result = false;
                                MyLogger.GetInstance.Error(string.Format("ZKDevice.DownloadLogData() - Error on device Ip: {0} ", this.Device.IP), e);
                            }

                            MyLogger.GetInstance.Info(string.Format("ZKDevice.DownloadLogData() - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dt.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                        }
                        else
                        {
                            MyLogger.GetInstance.Error(string.Format("ZKDevice.DownloadLogData() - Employee Not Exist - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dt.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                        }
                    }
                    else
                    {
                        result = false;
                        MyLogger.GetInstance.Error(string.Format("ZKDevice.DownloadLogData() - Time error - Insert into dbo.AttendanceRecord(DeviceId,EmployeeId,Date,InsertedDate) Values({0},{1},'{2}','{3}')", this.Device.DeviceId, idwEnrollNumber.ToString(), dtStr, DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                    }
                }          
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                result = false;

                if (idwErrorCode != 0)
                {
                    MyLogger.GetInstance.Error("ZKDevice.DownloadLogData() - Reading data from terminal failed, ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    MyLogger.GetInstance.Error("ZKDevice.DownloadLogData() - No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            MyLogger.GetInstance.Debug("ZKDevice.DownloadLogData() - Download: " + iGLCount + " records");

            return result;
        }

        private bool LoadRegisters()
        {
            bool result = false;

            MyLogger.GetInstance.Info("ZKDevice.LoadRegisters() - The process for download records is currently running");

            ConnectDevice();

            if (bIsConnected)
            {
                if (this.Device.IsSSR)
                {
                    result = SSRDownloadLogData();
                }
                else
                {
                    result = DownloadLogData();
                }

                DisconnectDevice();
            }

            return result;
        }

        public bool SyncTime()
        {
            bool result = false;

            MyLogger.GetInstance.Info("ZKDevice.SyncTime() Device IP: " + this.Device.IP);
            MyLogger.GetInstance.Info("ZKDevice.SyncTime() - The process for Sync devices is currently running");

            ConnectDevice();

            if (bIsConnected)
            {
                int idwErrorCode = 0;

                //this line get the local time and update the biometric device 
                if (axCZKEM1.SetDeviceTime(iMachineNumber))
                {
                    MyLogger.GetInstance.Debug("ZKDevice.SyncTime() - Syncing ZK devices at " + DateTime.Now);

                    if (axCZKEM1.RefreshData(iMachineNumber)) //the data in the device should be refreshed
                        MyLogger.GetInstance.Debug("ZKDevice.SyncTime() - Successfully set the time of the machine and the terminal to sync PC! ");

                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;

                    result = true;

                    try
                    {
                        if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                        {
                            var device = context.Devices.Find(Device.DeviceId);

                            device.SyncDate = DateTime.Parse(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());

                            //var device = context.Devices.Find(de)
                            context.Entry(device).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        MyLogger.GetInstance.Error("ZKDevice.SyncTime() - Sync operation failed ", e);
                    }
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    MyLogger.GetInstance.Error("ZKDevice.SyncTime() - Sync operation failed, ErrorCode=" + idwErrorCode.ToString());
                }
            }

            DisconnectDevice();

            return result;
        }

        public bool TransferRecords()
        {
            bool result = false;

            MyLogger.GetInstance.Info("ZKDevice.TransferRecords() - The process for Transfer Records is currently running");

            result = LoadRegisters();

            return result;
        }

        public void GetStatus()
        {
            MyLogger.GetInstance.Info("ZKDevice.GetStatus() - Device IP: " + this.Device.IP);

            try
            {
                var device = context.Devices.Find(this._device.DeviceId);

                if (device == null)
                    return;

                if (axCZKEM1.Connect_Net(this.Device.IP, this.Device.Port))
                {
                    device.DeviceStatus = DeviceStatus.Available;

                    iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)

                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;

                    if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                    {
                        device.SyncDate = DateTime.Parse(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    }
                }
                else
                {
                    device.DeviceStatus = DeviceStatus.Unavailable;
                }

                context.Entry(device).State = EntityState.Modified;
                context.SaveChanges();

            }
            catch (Exception e)
            {
                MyLogger.GetInstance.Error(string.Format("ZKDevice.GetStatus() - Error to get status on IP: ", this.Device.IP), e);
            }

            DisconnectDevice();
        }

        public bool ClearDevice()
        {
            MyLogger.GetInstance.Info("ZKDevice.ClearDevice() - Clear Device IP: " + this.Device.IP);

            bool result = false;

            ConnectDevice();

            if (bIsConnected == false)
                return result;

            int idwErrorCode = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ClearGLog(iMachineNumber))
            {
                if (axCZKEM1.RefreshData(iMachineNumber))
                {
                    //the data in the device should be refreshed
                    result = true;
                    MyLogger.GetInstance.Debug("ZKDevice.ClearDevice() - All att Logs have been cleared from teiminal!.");
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MyLogger.GetInstance.Debug("ZKDevice.ClearDevice() - Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device

            return result;
        }

        public void Dispose()
        {
            MyLogger.GetInstance.Info("ZKDevice.Dispose()");
            ((IDisposable)context).Dispose();
        }
    }
}
