using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.Models;
using System.Data.Entity;

namespace AttendanceRRHH.DAL
{
    public class TimeSheetRepository : ITimeSheetRepository, IDisposable
    {
        private ApplicationDbContext context;
        private bool disposed = false;

        public TimeSheetRepository()
        {
            context = new ApplicationDbContext();
        }

        public TimeSheetRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Delete(int timeSheetId)
        {
            TimeSheet timesheet = context.TimeSheets.Find(timeSheetId);
            context.TimeSheets.Remove(timesheet);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TimeSheet GetTimeSheetById(int timeSheetId)
        {
            return context.TimeSheets.Find(timeSheetId);
        }

        public void Insert(TimeSheet timesheet)
        {
            context.TimeSheets.Add(timesheet);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public IList<TimeSheet> TimeSheetList()
        {
            return context.TimeSheets.ToList();
        }

        public IList<TimeSheet> TimeSheetListByDate(DateTime date)
        {
            return context.TimeSheets.Where(w => w.Date == date).ToList();
        }

        public void Update(TimeSheet timesheet)
        {
            context.Entry(timesheet).State = EntityState.Modified;
        }

        public void InsertRange(IList<TimeSheet> timesheets)
        {
            context.TimeSheets.AddRange(timesheets);
        }

        public void DeleteByDate(DateTime date)
        {
            Func<TimeSheet, bool> filter = f => f.Date.Year == date.Year && f.Date.Month == date.Month && f.Date.Day == date.Day;
            var timesheets = context.TimeSheets.Where(filter);
            context.TimeSheets.RemoveRange(timesheets);
        }
    }
}