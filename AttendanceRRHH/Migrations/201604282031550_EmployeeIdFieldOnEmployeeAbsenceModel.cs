namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeIdFieldOnEmployeeAbsenceModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeeAbsences", "Employee_EmployeeId", "dbo.Employees");
            DropIndex("dbo.EmployeeAbsences", new[] { "Employee_EmployeeId" });
            RenameColumn(table: "dbo.EmployeeAbsences", name: "Employee_EmployeeId", newName: "EmployeeId");
            AlterColumn("dbo.EmployeeAbsences", "EmployeeId", c => c.Int(nullable: false));
            CreateIndex("dbo.EmployeeAbsences", "EmployeeId");
            AddForeignKey("dbo.EmployeeAbsences", "EmployeeId", "dbo.Employees", "EmployeeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeAbsences", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.EmployeeAbsences", new[] { "EmployeeId" });
            AlterColumn("dbo.EmployeeAbsences", "EmployeeId", c => c.Int());
            RenameColumn(table: "dbo.EmployeeAbsences", name: "EmployeeId", newName: "Employee_EmployeeId");
            CreateIndex("dbo.EmployeeAbsences", "Employee_EmployeeId");
            AddForeignKey("dbo.EmployeeAbsences", "Employee_EmployeeId", "dbo.Employees", "EmployeeId");
        }
    }
}
