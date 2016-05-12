namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtraHourPayFieldOnEmployeeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "IsExtraHourPay", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "IsExtraHourPay");
        }
    }
}
