using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.DAL
{
    public class AttendanceRecordsRepository : IAttendanceRecordsRepository, IDisposable
    {
        private AttendanceContext context;

        public AttendanceRecordsRepository()
        {
            context = new AttendanceContext();
        }

        public AttendanceRecordsRepository(AttendanceContext context)
        {
            this.context = context;
        }

        public IList<TimeSheet> GetEmployeeTimeSheet(DateTime initial, DateTime final)
        {

             var attendance = context.AttendanceRecords
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .Select(s => new
                {
                    EmployeeId = s.Key,
                    In = s.FirstOrDefault().Date,
                    Out = s.LastOrDefault().Date
                });

            var timesheet = attendance
                .Select(s => new TimeSheet() {
                    EmployeeId = s.EmployeeId,
                    Date = s.In,
                    In = s.In,
                    Out = s.Out,
                    IsManualIn = false,
                    IsManualOut = false,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true });

            return timesheet.ToList();
        }

        public IList<AttendanceRecord> ListAll()
        {
            var result = from a in context.AttendanceRecords
                         select a;

            return result.ToList();
        }

        public IList<AttendanceRecord> ListByDate(DateTime date)
        {
            var result = from a in context.AttendanceRecords
                         where a.Date.Year == date.Year &&
                               a.Date.Month == date.Month  &&
                               a.Date.Day == date.Day                      
                         select a;

            return result.ToList();
        }

        public IList<AttendanceRecord> ListByDateRange(DateTime initial, DateTime final)
        {
            var result = from a in context.AttendanceRecords
                         where initial >= a.Date && final <= a.Date
                         select a;

            return result.ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<AttendanceRecord> GetAttendaceByEmployee(int employeeId)
        {
            return context.AttendanceRecords.Where(w => w.EmployeeId == employeeId).ToList();
        }
        #endregion
    }
}