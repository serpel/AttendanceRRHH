namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtraHourFieldOnShiftModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours");
            DropIndex("dbo.ShiftTimes", new[] { "ExtraHourId" });
            AddColumn("dbo.Shifts", "ExtraHourId", c => c.Int());
            AddColumn("dbo.ExtraHourDetails", "Day", c => c.Int(nullable: false));
            CreateIndex("dbo.Shifts", "ExtraHourId");
            AddForeignKey("dbo.Shifts", "ExtraHourId", "dbo.ExtraHours", "ExtraHourId");
            DropColumn("dbo.ShiftTimes", "ExtraHourId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShiftTimes", "ExtraHourId", c => c.Int());
            DropForeignKey("dbo.Shifts", "ExtraHourId", "dbo.ExtraHours");
            DropIndex("dbo.Shifts", new[] { "ExtraHourId" });
            DropColumn("dbo.ExtraHourDetails", "Day");
            DropColumn("dbo.Shifts", "ExtraHourId");
            CreateIndex("dbo.ShiftTimes", "ExtraHourId");
            AddForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours", "ExtraHourId");
        }
    }
}
