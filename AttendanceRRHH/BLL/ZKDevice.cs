using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttendanceRRHH.Models;
using log4net;

namespace AttendanceRRHH.BLL
{
    public class ZKDevice : IDevice
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        public string Description { get; set;  }

        public string Ip { get; set; }

        public string Location { get; set; }

        public int Port { get; set; }

        public Status Status { get; set; }

        public DateTime Time { get; set; }

        public IList<AttendanceRecord> RecordList { get; set; }

        public bool IsSSR { get; set; }

        public string Type { get; set; }

        public int Id{ get; set; }
        private readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region ZKLibrary definitions
        private zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();
        private bool bIsConnected = false;
        private int iMachineNumber = 1;
        #endregion

        public ZKDevice()
        {
            RecordList = new List<AttendanceRecord>();
        }

        private void ConnectDevice()
        {
            if (bIsConnected) {
                this.Status = Status.Available;
                return;
            }

            if (this.Ip == null || this.Port < 0 || this.bIsConnected)
            {
                this.Status = Status.Unknown;
                logger.Error("ZKDevice.ConnectDevice() - IP and Port cannot be null");
                return;
            }
            int idwErrorCode = 0;

            bIsConnected = axCZKEM1.Connect_Net(this.Ip, this.Port);
            if (bIsConnected)
            {
                this.Status = Status.Available;
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                logger.Info("ZKDevice.ConnectDevice() - The device is connected successull "+this.Ip);
            }
            else
            {
                this.Status = Status.Unavailable;
                axCZKEM1.GetLastError(ref idwErrorCode);
                logger.Error("ZKDevice.ConnectDevice() - Unable to connect the device, ErrorCode=" + idwErrorCode.ToString());
            }
        }

        private void DisconnectDevice()
        {
            this.axCZKEM1.Disconnect();
            this.bIsConnected = false;
            logger.Info("ZKDevice.DisconnectDevice() - Device Disconnected: " + this.Ip);
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

        private void SSRDownloadLogData()
        {
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
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out idwEnrollNumber,
                       out idwVerifyMode, out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idWorkCode))//get records from the memory
                {
                    try { 
                        iGLCount++;
                        DateTime recordDate = DateTime.Parse(idwYear.ToString() + '-' + idwMonth.ToString() + '-' + idwDay.ToString() + ' ' + idwHour.ToString() + ':' + idwMinute.ToString());
                        //DateTime LocalDate = DateTime.Now;

                        var employee = context.Employees
                            .Where(w => w.EmployeeCode == idwEnrollNumber).FirstOrDefault();

                        var device = context.Devices
                            .Where(w => w.Description == this.Description).FirstOrDefault();

                        AttendanceRecord record = new AttendanceRecord()
                        {
                            EmployeeId = employee.EmployeeId,
                            Date = recordDate,
                            DeviceId = device.DeviceId,
                            InsertedAt = DateTime.Now
                        };

                        this.RecordList.Add(record);

                        logger.Info("ZKDevice.SSRDownloadLogData() - bio_Marcaciones2(" + idwEnrollNumber.ToString() + ", " + recordDate.ToString() + ", " + this.Location + " )");
                    }
                    catch (Exception e)
                    {
                        logger.Error("ZKDevice.SSRDownloadLogData() - Error ", e);
                    }
                }
                logger.Debug("ZKDevice.SSRDownloadLogData() - Download: "+iGLCount+" records");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    logger.Error("ZKDevice.SSRDownloadLogData() - Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    logger.Error("ZKDevice.SSRDownloadLogData() - No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true); //enable the device
        }

        private void DownloadLogData()
        {
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
                while (axCZKEM1.GetGeneralLogData(iMachineNumber, ref idwTMachineNumber, ref idwEnrollNumber,
                        ref idwEMachineNumber, ref idwVerifyMode, ref idwInOutMode, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute))//get records from the memory
                {
                    try { 
                        iGLCount++;
                        DateTime recordDate = DateTime.Parse(idwYear.ToString() + '-' + idwMonth.ToString() + '-' + idwDay.ToString() + ' ' + idwHour.ToString() + ':' + idwMinute.ToString());
                        //DateTime LocalDate = DateTime.Now;


                        var employee = context.Employees
                            .Where(w => w.EmployeeCode == idwEnrollNumber.ToString()).FirstOrDefault();

                        var device = context.Devices
                            .Where(w => w.Description == this.Description).FirstOrDefault();

                        AttendanceRecord record = new AttendanceRecord()
                        {
                            EmployeeId = employee.EmployeeId,
                            Date = recordDate,
                            DeviceId = device.DeviceId,
                            InsertedAt = DateTime.Now
                        };

                        RecordList.Add(record);

                        logger.Info("ZKDevice.DownloadLogData() - bio_Marcaciones2(" + idwEnrollNumber.ToString() + ", " + recordDate.ToString() + ", "+ this.Location +" )");
                    }
                    catch(Exception e)
                    {
                        logger.Error("ZKDevice.DownloadLogData() - Error ", e);
                    }
                }
                logger.Debug("ZKDevice.DownloadLogData() - Download: " + iGLCount + " records");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    logger.Error("ZKDevice.DownloadLogData() - Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    logger.Error("ZKDevice.DownloadLogData() - No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }

        private void LoadRegisters()
        {
            logger.Info("ZKDevice.LoadRegisters() - The process for download records is currently running");

            ConnectDevice();

            if (bIsConnected)
            {
                if(IsSSR)
                {
                    SSRDownloadLogData();
                }
                else
                {
                    DownloadLogData();
                }
            }

            DisconnectDevice();
        }

        public bool SyncTime()
        {
            bool result = false;

            logger.Info("ZKDevice.SyncTime() - The process for Sync devices is currently running");

            ConnectDevice();

            if (bIsConnected)
            {
                int idwErrorCode = 0;

                //this line get the local time and update the biometric device 
                if (axCZKEM1.SetDeviceTime(iMachineNumber))
                {
                    logger.Debug("ZKDevice.SyncTime() - Syncing ZK devices at " + DateTime.Now);

                    if (axCZKEM1.RefreshData(iMachineNumber)) //the data in the device should be refreshed
                        logger.Debug("ZKDevice.SyncTime() - Successfully set the time of the machine and the terminal to sync PC! ");

                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;

                    result = true;

                    try { 
                        if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                        {
                            this.Time = DateTime.Parse(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                        }
                    }catch(Exception e)
                    {
                        logger.Error("ZKDevice.SyncTime() - Sync operation failed ", e);
                    }
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    logger.Error("ZKDevice.SyncTime() - Sync operation failed, ErrorCode=" + idwErrorCode.ToString());
                }
            }

            DisconnectDevice();

            return result;
        }

        public bool TransferRecords()
        {
            bool result = false;

            logger.Info("ZKDevice.TransferRecords() - The process for Transfer Records is currently running");

            LoadRegisters();

            if(this.RecordList != null)
            {
                try {
                    context.AttendanceRecords.AddRange(this.RecordList);
                    context.SaveChanges();
                    result = true;

                    logger.Debug("ZKDevice.TransferRecords() - " + this.RecordList.Count + " will be saved on database");
                }catch(Exception e)
                {
                    logger.Error("ZKDevice.TransferRecords() - Unable to transfer records", e);
                }
            }

            return result;
        }

        public void GetStatus()
        {
            logger.Info("ZKDevice.GetStatus() - get device status and time ");

            try {
                 
                if (axCZKEM1.Connect_Net(this.Ip, this.Port))
                {
                    this.Status = Status.Available;
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
                        this.Time = DateTime.Parse(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    }
                }
                else
                {
                    this.Status = Status.Unavailable;
                }
            }catch(Exception e)
            {
                logger.Error("ZKDevice.GetStatus() - Error to get status ", e);
            }

            DisconnectDevice();
        }

        public bool ClearDevice()
        {
            logger.Info("ZKDevice.ClearDevice() - Clear Device");

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
                    logger.Debug("ZKDevice.ClearDevice() - All att Logs have been cleared from teiminal!.");
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                logger.Debug("ZKDevice.ClearDevice() - Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device

            return result;
        }
    }
}
