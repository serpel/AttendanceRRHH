namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceStatusAndSyncCronExpressionAndSyncDateFieldsOnDevicesModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "SyncCronExpression", c => c.String());
            AddColumn("dbo.Devices", "DeviceStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Devices", "SyncDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "SyncDate");
            DropColumn("dbo.Devices", "DeviceStatus");
            DropColumn("dbo.Devices", "SyncCronExpression");
        }
    }
}
