using AttendanceRRHH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.BLL
{
    public interface IDevice
    {
        Device Device { get; set; }

        bool SyncTime();

        bool ClearDevice();

        bool TransferRecords();

        void GetStatus();
    }
}
