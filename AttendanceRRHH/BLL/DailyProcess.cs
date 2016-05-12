using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using AttendanceRRHH.Models;
using AttendanceRRHH.DAL;

namespace AttendanceRRHH.BLL
{
    public class DailyProcess
    {
        private AttendanceContext context;
        private EmployeeRepository eRepository;
        private ShiftTimeRepository sRepository;
        private AttendanceRecordsRepository aRepository;
        private TimeSheetRepository tRepository;

        public DailyProcess():this(new AttendanceContext()) { }

        public DailyProcess(AttendanceContext context)
        {
            this.context = context;
            eRepository = new EmployeeRepository(context);
            sRepository = new ShiftTimeRepository(context);
            aRepository = new AttendanceRecordsRepository(context);
            tRepository = new TimeSheetRepository(context);
        }

        public void GenerateEmployeeTimeSheetByDate(DateTime date)
        {
            tRepository.DeleteByDate(date);
            tRepository.Save();

            var employees = eRepository.GetAll().Where(w => w.IsActive == true);

            foreach(var employee in employees)
            {
                //func expresion para filtrar una fecha especifica
                Func<AttendanceRecord, bool> filter = f => f.Date.Year == date.Year && f.Date.Month == date.Month && f.Date.Day == date.Day;
                var attendances = aRepository.GetAttendaceByEmployee(employee.EmployeeId).Where(filter);
                var shifttime = sRepository.GetListByShiftIdAndDayNumber(employee.ShiftId, ((int)date.DayOfWeek + 1));

                //si tiene marcas hay que repararlas, si no hay que agregar la fecha 
                if(attendances.Count() > 0)
                {
                    var employeeTimesheet = attendances
                             .GroupBy(x => x.EmployeeId)
                             .Select(s => new TimeSheet()
                             {
                                 EmployeeId = s.Key,
                                 Date = date,
                                 In = s.FirstOrDefault().Date,
                                 Out = s.LastOrDefault().Date,
                                 IsManualIn = false,
                                 IsManualOut = false,
                                 InsertedAt = DateTime.Now,
                                 UpdatedAt = DateTime.Now,
                                 IsActive = true,
                                 ShiftTimeId = shifttime.ShiftTimeId
                             }).FirstOrDefault();
                    //El tiempo para considerar una marca es de una hora si no se reemplaza por la que tiene el turno
                    TimeSpan overtime = TimeSpan.FromHours(1);
                    TimeSpan checkin = employeeTimesheet.In.Value.TimeOfDay;
                    TimeSpan checkout = employeeTimesheet.Out.Value.TimeOfDay;

                    //arreglo las horas de entrada y salida basado en el turno del empleado
                    if (!(checkin <= (shifttime.StartTime + overtime)))
                    {
                        employeeTimesheet.In = new DateTime(
                        employeeTimesheet.In.Value.Year,
                        employeeTimesheet.In.Value.Month,
                        employeeTimesheet.In.Value.Day,
                        shifttime.StartTime.Hours,
                        shifttime.StartTime.Minutes,
                        shifttime.StartTime.Seconds);
                        employeeTimesheet.UpdatedAt = DateTime.Now;
                        employeeTimesheet.IsManualIn = true;
                    }

                    //checkout <= (shiftday.EndTime + overtime) &&
                    if (!(checkout >= (shifttime.EndTime - overtime)))
                    {
                        employeeTimesheet.Out = new DateTime(employeeTimesheet.Out.Value.Year,
                        employeeTimesheet.Out.Value.Month,
                        employeeTimesheet.Out.Value.Day,
                        shifttime.EndTime.Hours,
                        shifttime.EndTime.Minutes,
                        shifttime.EndTime.Seconds);
                        employeeTimesheet.UpdatedAt = DateTime.Now;
                        employeeTimesheet.IsManualOut = true;
                    }

                    tRepository.Insert(employeeTimesheet);
                }
                else
                {
                    TimeSheet timesheet = new TimeSheet()
                    {
                        EmployeeId = employee.EmployeeId,
                        Date = date,
                        IsManualIn = true,
                        IsManualOut = true,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsActive = true
                    };
                    tRepository.Insert(timesheet);
                }

                tRepository.Save();
            }
        }

        public void GenerateEmployeeTimeSheetByDay(DateTime date)
        {
            tRepository.DeleteByDate(date);
            tRepository.Save();

            var employees = context.Employees.Include(i => i.Shift).Include(i => i.Schedules).Include(i => i.AttendanceRecords)
                .Where(w => w.IsActive == true).ToList();

            foreach(var employee in employees)
            {
                //saco la lista de cambios de turnos de una fecha en especifico 
                var schedule = employee
                    .Schedules
                    .Where(w => date >= w.StartDate && date <= w.EndDate)
                    .FirstOrDefault();

                // reviso si hay un cambio de turno y reemplazo los tiempo del turno
                ShiftTime shifttime;              
                if (schedule != null)
                {
                     shifttime = schedule
                     .Shift
                     .ShiftTimes
                     .Where(w => w.DayNumber == (int)date.DayOfWeek)
                     .FirstOrDefault();
                }
                else
                {
                    shifttime = employee
                    .Shift
                    .ShiftTimes
                    .Where(w => w.DayNumber == (int)date.DayOfWeek)
                    .FirstOrDefault();
                }

                if (shifttime == null)
                    continue;

                TimeSpan overtime = TimeSpan.FromHours(3);
                DateTime start = date.toDateTime(shifttime.StartTime - overtime);
                DateTime end = date.toDateTime(shifttime.EndTime + overtime);

                //si el end date termina al dia siguiente
                if (start >= end)
                    end = end.AddDays(1);

                TimeSheet timesheet = employee.AttendanceRecords
                    .Where(w => w.Date >= start && w.Date <= end)
                    .GroupBy(g => g.EmployeeId)
                    .Select(s => new TimeSheet()
                    {
                        EmployeeId = s.Key,
                        Date = date,
                        In = s.FirstOrDefault().Date,
                        Out = s.LastOrDefault().Date,
                        IsManualIn = false,
                        IsManualOut = false,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsActive = true,
                        ShiftTimeId = shifttime.ShiftTimeId
                    }).FirstOrDefault();


                if (timesheet == null)
                {
                    context.TimeSheets.Add(
                        new TimeSheet()
                        {
                            EmployeeId = employee.EmployeeId,
                            Date = date,
                            IsManualIn = true,
                            IsManualOut = true,
                            InsertedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true,
                            ShiftTimeId = shifttime.ShiftTimeId                           
                        });
                }
                else
                {
                    //El tiempo para considerar una marca es de una hora si no se reemplaza por la que tiene el turno
                    TimeSpan checkin = timesheet.In.Value.TimeOfDay;
                    TimeSpan checkout = timesheet.Out.Value.TimeOfDay;

                    //arreglo las horas de entrada y salida basado en el turno del empleado
                    if (timesheet.In.Value >= timesheet.In.Value.toDateTime(shifttime.StartTime + overtime))
                    {
                        timesheet.In = timesheet.In.Value.toDateTime(shifttime.StartTime);
                        timesheet.UpdatedAt = DateTime.Now;
                        timesheet.IsManualIn = true;
                    }

                    DateTime tmp = end.toDateTime(end.TimeOfDay - (overtime+overtime));
                    if (timesheet.Out <= tmp)
                    {
                        timesheet.Out = end.toDateTime(shifttime.EndTime);
                        timesheet.UpdatedAt = DateTime.Now;
                        timesheet.IsManualOut = true;
                    }

                    context.TimeSheets.Add(timesheet);
                }
            }
            context.SaveChanges();
        }

        public void GenerateEmployeeTimeSheetByDayAndCompany(DateTime date, int companyId)
        {
            //borrar data vieja
            Func<TimeSheet, bool> filter = f => f.Date.Year == date.Year
            && f.Date.Month == date.Month
            && f.Date.Day == date.Day
            && f.Employee.Department.CompanyId == companyId;

            var timesheets = context.TimeSheets.Where(filter);
            context.TimeSheets.RemoveRange(timesheets);

            var employees = context.Employees.Include(i => i.Shift).Include(i => i.Schedules).Include(i => i.AttendanceRecords)
                .Where(w => w.IsActive == true 
                       && w.Department.CompanyId == companyId)
                .ToList();

            foreach (var employee in employees)
            {
                //saco la lista de cambios de turnos de una fecha en especifico 
                var schedule = employee
                    .Schedules
                    .Where(w => date >= w.StartDate && date <= w.EndDate)
                    .FirstOrDefault();

                // reviso si hay un cambio de turno y reemplazo los tiempo del turno
                ShiftTime shifttime;
                if (schedule != null)
                {
                    shifttime = schedule
                    .Shift
                    .ShiftTimes
                    .Where(w => w.DayNumber == (int)date.DayOfWeek)
                    .FirstOrDefault();
                }
                else
                {
                    shifttime = employee
                    .Shift
                    .ShiftTimes
                    .Where(w => w.DayNumber == (int)date.DayOfWeek)
                    .FirstOrDefault();
                }

                if (shifttime == null)
                    continue;

                TimeSpan overtime = TimeSpan.FromHours(3);
                DateTime start = date.toDateTime(shifttime.StartTime - overtime);
                DateTime end = date.toDateTime(shifttime.EndTime + overtime);

                //si el end date termina al dia siguiente
                if (start >= end)
                    end = end.AddDays(1);

                TimeSheet timesheet = employee.AttendanceRecords
                    .Where(w => w.Date >= start && w.Date <= end)
                    .GroupBy(g => g.EmployeeId)
                    .Select(s => new TimeSheet()
                    {
                        EmployeeId = s.Key,
                        Date = date,
                        In = s.FirstOrDefault().Date,
                        Out = s.LastOrDefault().Date,
                        IsManualIn = false,
                        IsManualOut = false,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsActive = true,
                        ShiftTimeId = shifttime.ShiftTimeId
                    }).FirstOrDefault();


                if (timesheet == null)
                {
                    context.TimeSheets.Add(
                        new TimeSheet()
                        {
                            EmployeeId = employee.EmployeeId,
                            Date = date,
                            IsManualIn = true,
                            IsManualOut = true,
                            InsertedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            IsActive = true,
                            ShiftTimeId = shifttime.ShiftTimeId
                        });
                }
                else
                {
                    //El tiempo para considerar una marca es de una hora si no se reemplaza por la que tiene el turno
                    TimeSpan checkin = timesheet.In.Value.TimeOfDay;
                    TimeSpan checkout = timesheet.Out.Value.TimeOfDay;

                    //arreglo las horas de entrada y salida basado en el turno del empleado
                    if (timesheet.In.Value >= timesheet.In.Value.toDateTime(shifttime.StartTime + overtime))
                    {
                        timesheet.In = timesheet.In.Value.toDateTime(shifttime.StartTime);
                        timesheet.UpdatedAt = DateTime.Now;
                        timesheet.IsManualIn = true;
                    }

                    DateTime tmp = end.toDateTime(end.TimeOfDay - (overtime + overtime));
                    if (timesheet.Out <= tmp)
                    {
                        timesheet.Out = end.toDateTime(shifttime.EndTime);
                        timesheet.UpdatedAt = DateTime.Now;
                        timesheet.IsManualOut = true;
                    }

                    context.TimeSheets.Add(timesheet);
                }
            }
            context.SaveChanges();
        }

        public void UpdateTimeSheetByEmployee(int employeeId)
        {
            var timesheets = context.TimeSheets
                .Where(w => w.EmployeeId == employeeId);

            var schedules = context.Schedules
               .Where(w => w.EmployeeId == employeeId);


            foreach (TimeSheet t in timesheets)
            {
                foreach (Schedule s in schedules)
                {
                    if (IsInRange(t.Date, s.StartDate, s.EndDate))
                    {
                        //Func<AttendanceRecord, bool> filter = f => f.Date.Year == t.Date.Year && f.Date.Month == t.Date.Month && f.Date.Day == t.Date.Day;
                        var attendances = context.AttendanceRecords
                            .Where(w => w.EmployeeId == employeeId &&
                                        w.Date.Year == t.Date.Year &&
                                        w.Date.Month == t.Date.Month &&
                                        w.Date.Day == t.Date.Day);

                        var shifttime = context.ShiftTimes
                            .Where(w => w.ShiftId == s.ShiftId &&
                                        w.DayNumber == ((int)t.Date.DayOfWeek + 1));

                        //if (attendances.Count() > 0)
                        //{
                        //    var employeeTimesheet = attendances
                        //             .GroupBy(x => x.EmployeeId)
                        //             .Select(s => new TimeSheet()
                        //             {
                        //                 EmployeeId = s.Key,
                        //                 Date = t.Date,
                        //                 In = s.FirstOrDefault().Date,
                        //                 Out = s.LastOrDefault().Date,
                        //                 IsManualIn = false,
                        //                 IsManualOut = false,
                        //                 InsertedAt = DateTime.Now,
                        //                 UpdatedAt = DateTime.Now,
                        //                 IsActive = true,
                        //                 ShiftTimeId = shifttime.ShiftTimeId
                        //             }).FirstOrDefault();
                        //}
                    }
                }
            }
        }

        public bool IsInRange(DateTime date, DateTime start, DateTime end)
        {
            bool result = false;

            if(start >= date && date <= end)
            {
                result = true;
            }

            return result;
        }
       
    }
}