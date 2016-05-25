namespace AttendanceRRHH.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AttendanceRRHH.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AttendanceRRHH.Models.ApplicationDbContext context)
        {
            context.Absences.AddOrUpdate(x => x.AbsenceId,
               new Absence() { AbsenceId = 1, Name = "Vacaciones", IsActive = true },
               new Absence() { AbsenceId = 2, Name = "Feriado", IsActive = true },
               new Absence() { AbsenceId = 3, Name = "Enfermedad", IsActive = true },
               new Absence() { AbsenceId = 4, Name = "Ausencia de Marca", IsActive = true },
               new Absence() { AbsenceId = 5, Name = "Otros", IsActive = true }
               );

            context.Jobs.AddOrUpdate(x => x.JobPositionId,
                 //new JobPosition() { JobPositionId = 6, JobTitle = "Consultor Desarrollo", IsActive = true },
                 //new JobPosition() { JobPositionId = 2, JobTitle = "Tecnico", IsActive = true },
                 //new JobPosition() { JobPositionId = 3, JobTitle = "Coodinador IT", IsActive = true },
                 //new JobPosition() { JobPositionId = 4, JobTitle = "Consultor SAP", IsActive = true },
                 //new JobPosition() { JobPositionId = 5, JobTitle = "Aseador", IsActive = true },
                 new JobPosition() { JobPositionId = 1, JobTitle = "Otros", IsActive = true }
             );

            context.Countries.AddOrUpdate(x => x.CountryId,
                new Country() { CountryId = 1, Name = "Honduras", IsActive = true },
                new Country() { CountryId = 2, Name = "Guatemala", IsActive = true },
                new Country() { CountryId = 3, Name = "Costa Rica", IsActive = true }
                );

            context.Cities.AddOrUpdate(x => x.CityId,
                new City() { CityId = 1, CountryId = 1, Name = "Choloma", IsActive = true },
                new City() { CityId = 2, CountryId = 1, Name = "Puerto Cortes", IsActive = true },
                new City() { CityId = 3, CountryId = 1, Name = "San Pedro Sula", IsActive = true }
                );

            //context.Companies.AddOrUpdate(x => x.CompanyId,
            //    new Company() { CompanyId = 1, CityId = 1, CountryId = 1, Name = "Inhdelva", Address = "Choloma Calle Jutosa", IsActive = true, LogoUrl = "" },
            //    new Company() { CompanyId = 3, CityId = 1, CountryId = 3, Name = "Italian", Address = "Choloma parque Inhdelva Norte", IsActive = true }
            //    );

            //context.Departments.AddOrUpdate(x => x.DepartmentId,
            //    new Department() { DepartmentId = 1, CompanyId = 1, Name = "Departamento de IT", IsActive = true },
            //    new Department() { DepartmentId = 2, CompanyId = 1, Name = "Mantenimiento", IsActive = true }
            //    );

            context.DeviceTypes.AddOrUpdate(x => x.DeviceTypeId,
                new DeviceType() { DeviceTypeId = 1, Name = "ZK", Description = "ZKTecno Device", IsActive = true }
                );

            //context.Devices.AddOrUpdate(x => x.DeviceId,
            //    new Device() { DeviceId = 1, DeviceTypeId = 1, Description = "Corporativo", Location = "Oficinas del Coporativo Inhdelva", IP = "172.20.11.20", Port = 4370, IsSSR = true, OpenDoors = true }
            //    );

            //context.Shifts.AddOrUpdate(x => x.ShiftId,
            //    new Shift() { ShiftId = 1, CompanyId = 1, Name = "Regular Shift", Description = "Turno regular de 8 - 5 PM", InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true }
            //    );

            //context.ShiftTime.AddOrUpdate(x => x.ShiftTimeId,
            //    new ShiftTime() { ShiftTimeId = 1, ShiftId = 1, DayNumber = 1, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("17:00"), IsLaborDay = true, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = true, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 2, ShiftId = 1, DayNumber = 2, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("17:00"), IsLaborDay = true, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = true, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 3, ShiftId = 1, DayNumber = 3, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("17:00"), IsLaborDay = true, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = true, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 4, ShiftId = 1, DayNumber = 4, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("17:00"), IsLaborDay = true, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = true, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 5, ShiftId = 1, DayNumber = 5, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("17:00"), IsLaborDay = true, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = true, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 6, ShiftId = 1, DayNumber = 6, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("12:00"), IsLaborDay = false, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = false, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true },
            //    new ShiftTime() { ShiftTimeId = 7, ShiftId = 1, DayNumber = 7, StartTime = TimeSpan.Parse("8:00"), EndTime = TimeSpan.Parse("12:00"), IsLaborDay = false, LunchStartTime = TimeSpan.Parse("12:00"), LunchEndTime = TimeSpan.Parse("13:00"), HasLunchTime = false, InsertedAt = DateTime.Now, UpdatedAt = DateTime.Now, IsActive = true }
            //    );

            //context.Employees.AddOrUpdate(x => x.EmployeeId,
            //    new Employee() { EmployeeId = 1, EmployeeCode = "17", FirstName = "Sergio", LastName = "Peralta", Gender = Gender.M, CityId = 2, CountryId = 1, DepartmentId = 1, HireDate = DateTime.Parse("2015-12-02"), JobPositionId = 1, IsActive = true, ShiftId = 1 },
            //    new Employee() { EmployeeId = 2, EmployeeCode = "1320", FirstName = "Jairo", LastName = "Espinza", Gender = Gender.M, CityId = 2, CountryId = 1, DepartmentId = 1, HireDate = DateTime.Parse("2015-12-02"), JobPositionId = 1, IsActive = true, ShiftId = 1 },
            //    new Employee() { EmployeeId = 3, EmployeeCode = "1321", FirstName = "Teresa", LastName = "Espinal", Gender = Gender.F, CityId = 2, CountryId = 1, DepartmentId = 1, HireDate = DateTime.Parse("2015-12-02"), JobPositionId = 1, IsActive = true, ShiftId = 1 },
            //    new Employee() { EmployeeId = 4, EmployeeCode = "1322", FirstName = "Alexander", LastName = "Gadiel", Gender = Gender.M, CityId = 1, CountryId = 1, DepartmentId = 1, HireDate = DateTime.Parse("2015-12-02"), JobPositionId = 1, IsActive = true, ShiftId = 1 }
            //    );

            //context.AttendanceRecords.AddOrUpdate(x => x.AttendanceRecodId,
            //   new AttendanceRecord() { AttendanceRecodId = 1, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 08:23"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 2, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 08:25"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 3, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 10:00"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 4, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 11:05"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 5, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 12:24"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 6, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 12:55"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 7, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 13:37"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 8, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 14:16"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 9, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 16:00"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 10, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 16:05"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 11, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 15:07"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 12, EmployeeId = 1, DeviceId = 1, Date = DateTime.Parse("2016-02-23 17:57"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 13, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 06:10"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 14, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 06:27"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 15, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 07:57"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 16, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 08:36"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 17, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 09:57"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 18, EmployeeId = 2, DeviceId = 1, Date = DateTime.Parse("2016-02-23 17:13"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 19, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 08:29"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 20, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 09:19"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 21, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 10:53"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 22, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 11:19"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 23, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 12:53"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 24, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 13:10"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 25, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 14:53"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 26, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 15:53"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 27, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 15:53"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 28, EmployeeId = 3, DeviceId = 1, Date = DateTime.Parse("2016-02-23 15:58"), InsertedAt = DateTime.Now },
            //   new AttendanceRecord() { AttendanceRecodId = 29, EmployeeId = 4, DeviceId = 1, Date = DateTime.Parse("2016-02-23 7:58"), InsertedAt = DateTime.Now }
            //   );
            Console.WriteLine("Finish");
            context.SaveChanges();
        }
    }
}
