namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Absences",
                c => new
                    {
                        AbsenceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AbsenceId);
            
            CreateTable(
                "dbo.EmployeeAbsences",
                c => new
                    {
                        EmployeeAbsenceId = c.Int(nullable: false, identity: true),
                        AbsenceId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Employee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeAbsenceId)
                .ForeignKey("dbo.Absences", t => t.AbsenceId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.Employee_EmployeeId)
                .Index(t => t.AbsenceId)
                .Index(t => t.Employee_EmployeeId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(nullable: false, maxLength: 20),
                        NationalCardId = c.String(maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Address = c.String(maxLength: 150),
                        Birthdate = c.DateTime(),
                        Gender = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        ProfileUrl = c.String(),
                        HireDate = c.DateTime(),
                        DepartmentId = c.Int(nullable: false),
                        ShiftId = c.Int(nullable: false),
                        JobPositionId = c.Int(),
                        CountryId = c.Int(),
                        CityId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.JobPositions", t => t.JobPositionId)
                .ForeignKey("dbo.Shifts", t => t.ShiftId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.ShiftId)
                .Index(t => t.JobPositionId)
                .Index(t => t.CountryId)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.AttendanceRecords",
                c => new
                    {
                        AttendanceRecodId = c.Int(nullable: false, identity: true),
                        DeviceId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        InsertedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttendanceRecodId)
                .ForeignKey("dbo.Devices", t => t.DeviceId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.DeviceId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        DeviceId = c.Int(nullable: false, identity: true),
                        DeviceTypeId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 150),
                        Location = c.String(maxLength: 150),
                        IP = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        IsSSR = c.Boolean(nullable: false),
                        OpenDoors = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceId)
                .ForeignKey("dbo.DeviceTypes", t => t.DeviceTypeId, cascadeDelete: true)
                .Index(t => t.DeviceTypeId);
            
            CreateTable(
                "dbo.DeviceTypes",
                c => new
                    {
                        DeviceTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DeviceTypeId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CountryId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Name = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 150),
                        LogoUrl = c.String(),
                        CountryId = c.Int(),
                        CityId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .Index(t => t.CountryId)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        ShiftId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 150),
                        IsSpecialShift = c.Boolean(nullable: false),
                        InsertedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ShiftId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        ScheduleId = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(),
                        EmployeeId = c.Int(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        InsertedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Shifts", t => t.ShiftId)
                .Index(t => t.ShiftId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.ShiftTimes",
                c => new
                    {
                        ShiftTimeId = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(nullable: false),
                        ExtraHourId = c.Int(nullable: false),
                        DayNumber = c.Int(nullable: false),
                        IsLaborDay = c.Boolean(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        HasLunchTime = c.Boolean(nullable: false),
                        LunchStartTime = c.Time(nullable: false, precision: 7),
                        LunchEndTime = c.Time(nullable: false, precision: 7),
                        InsertedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ShiftTimeId)
                .ForeignKey("dbo.ExtraHours", t => t.ExtraHourId, cascadeDelete: true)
                .ForeignKey("dbo.Shifts", t => t.ShiftId, cascadeDelete: true)
                .Index(t => t.ShiftId)
                .Index(t => t.ExtraHourId);
            
            CreateTable(
                "dbo.ExtraHours",
                c => new
                    {
                        ExtraHourId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.ExtraHourId);
            
            CreateTable(
                "dbo.ExtraHourDetails",
                c => new
                    {
                        ExtraHourDetailId = c.Int(nullable: false, identity: true),
                        ExtraHourId = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        StartHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ExtraHourDetailId)
                .ForeignKey("dbo.ExtraHours", t => t.ExtraHourId, cascadeDelete: true)
                .Index(t => t.ExtraHourId);
            
            CreateTable(
                "dbo.JobPositions",
                c => new
                    {
                        JobPositionId = c.Int(nullable: false, identity: true),
                        JobTitle = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.JobPositionId);
            
            CreateTable(
                "dbo.TimeSheets",
                c => new
                    {
                        TimeSheetId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ShiftTimeId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        In = c.DateTime(),
                        Out = c.DateTime(),
                        IsManualIn = c.Boolean(nullable: false),
                        IsManualOut = c.Boolean(nullable: false),
                        InsertedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TimeSheetId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.ShiftTimes", t => t.ShiftTimeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ShiftTimeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeSheets", "ShiftTimeId", "dbo.ShiftTimes");
            DropForeignKey("dbo.TimeSheets", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.Employees", "JobPositionId", "dbo.JobPositions");
            DropForeignKey("dbo.EmployeeAbsences", "Employee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.ShiftTimes", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours");
            DropForeignKey("dbo.ExtraHourDetails", "ExtraHourId", "dbo.ExtraHours");
            DropForeignKey("dbo.Schedules", "ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.Schedules", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Shifts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Departments", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Companies", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Employees", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Employees", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.AttendanceRecords", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Devices", "DeviceTypeId", "dbo.DeviceTypes");
            DropForeignKey("dbo.AttendanceRecords", "DeviceId", "dbo.Devices");
            DropForeignKey("dbo.EmployeeAbsences", "AbsenceId", "dbo.Absences");
            DropIndex("dbo.TimeSheets", new[] { "ShiftTimeId" });
            DropIndex("dbo.TimeSheets", new[] { "EmployeeId" });
            DropIndex("dbo.ExtraHourDetails", new[] { "ExtraHourId" });
            DropIndex("dbo.ShiftTimes", new[] { "ExtraHourId" });
            DropIndex("dbo.ShiftTimes", new[] { "ShiftId" });
            DropIndex("dbo.Schedules", new[] { "EmployeeId" });
            DropIndex("dbo.Schedules", new[] { "ShiftId" });
            DropIndex("dbo.Shifts", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "CityId" });
            DropIndex("dbo.Companies", new[] { "CountryId" });
            DropIndex("dbo.Departments", new[] { "CompanyId" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropIndex("dbo.Devices", new[] { "DeviceTypeId" });
            DropIndex("dbo.AttendanceRecords", new[] { "EmployeeId" });
            DropIndex("dbo.AttendanceRecords", new[] { "DeviceId" });
            DropIndex("dbo.Employees", new[] { "CityId" });
            DropIndex("dbo.Employees", new[] { "CountryId" });
            DropIndex("dbo.Employees", new[] { "JobPositionId" });
            DropIndex("dbo.Employees", new[] { "ShiftId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropIndex("dbo.EmployeeAbsences", new[] { "Employee_EmployeeId" });
            DropIndex("dbo.EmployeeAbsences", new[] { "AbsenceId" });
            DropTable("dbo.TimeSheets");
            DropTable("dbo.JobPositions");
            DropTable("dbo.ExtraHourDetails");
            DropTable("dbo.ExtraHours");
            DropTable("dbo.ShiftTimes");
            DropTable("dbo.Schedules");
            DropTable("dbo.Shifts");
            DropTable("dbo.Companies");
            DropTable("dbo.Departments");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
            DropTable("dbo.DeviceTypes");
            DropTable("dbo.Devices");
            DropTable("dbo.AttendanceRecords");
            DropTable("dbo.Employees");
            DropTable("dbo.EmployeeAbsences");
            DropTable("dbo.Absences");
        }
    }
}
