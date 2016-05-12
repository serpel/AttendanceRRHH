using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.DAL
{
    public interface IAttendanceRecordsRepository: IDisposable
    {
        IList<AttendanceRecord> ListAll();
        IList<AttendanceRecord> ListByDate(DateTime date);
        IList<AttendanceRecord> ListByDateRange(DateTime initial, DateTime final);
        List<AttendanceRecord> GetAttendaceByEmployee(int employeeId);
        IList<TimeSheet> GetEmployeeTimeSheet(DateTime initial, DateTime final); 
    }
}