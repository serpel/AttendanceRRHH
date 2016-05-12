using System.Data.Entity;

namespace AttendanceRRHH.Models
{
    public class AttendanceContext: DbContext
    {
        public AttendanceContext() : base("DefaultConnection") { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobPosition> Jobs { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ShiftTime> ShiftTimes { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<EmployeeAbsence> EmployeeAbsences { get; set; }
        public DbSet<ExtraHour> ExtraHours { get; set; }
        public DbSet<ExtraHourDetail> ExtraHourDetails { get; set; }
    }
}