using AttendanceRRHH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRRHH.DAL
{
    interface ITimeSheetRepository: IDisposable
    {
        void Insert(TimeSheet timesheet);
        void InsertRange(IList<TimeSheet> timesheets);
        void Update(TimeSheet timesheet);
        void Delete(int timeSheetId);
        void DeleteByDate(DateTime date);
        TimeSheet GetTimeSheetById(int timeSheetId);
        IList<TimeSheet> TimeSheetList();
        IList<TimeSheet> TimeSheetListByDate(DateTime date);
        void Save();
    }
}
