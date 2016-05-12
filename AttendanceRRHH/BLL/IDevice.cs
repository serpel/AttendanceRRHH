using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.BLL
{
    public enum Status
    {
        Available = 1,
        Unavailable = 0,
        Unknown = 2,
    }

    public interface IDevice
    {
        int Id { get; set; }

        String Ip { get; set; }

        int Port { get; set; }

        String Description { get; set; }

        String Location { get; set; }

        String Type { get; set; }

        Status Status { get; set; }

        DateTime Time { get; set; }

        bool IsSSR { get; set; }

        bool SyncTime();

        bool ClearDevice();

        bool TransferRecords();

        void GetStatus();
    }
}
