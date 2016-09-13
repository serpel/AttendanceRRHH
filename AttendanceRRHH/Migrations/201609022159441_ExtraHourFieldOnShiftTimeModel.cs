namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtraHourFieldOnShiftTimeModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours");
            DropIndex("dbo.ShiftTimes", new[] { "ExtraHourId" });
            AlterColumn("dbo.ShiftTimes", "ExtraHourId", c => c.Int());
            CreateIndex("dbo.ShiftTimes", "ExtraHourId");
            AddForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours", "ExtraHourId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours");
            DropIndex("dbo.ShiftTimes", new[] { "ExtraHourId" });
            AlterColumn("dbo.ShiftTimes", "ExtraHourId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShiftTimes", "ExtraHourId");
            AddForeignKey("dbo.ShiftTimes", "ExtraHourId", "dbo.ExtraHours", "ExtraHourId", cascadeDelete: true);
        }
    }
}
