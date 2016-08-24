namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncTimeExprAndTransferCronExprFieldsOnDevicesModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "SyncTimeCronExpression", c => c.String());
            AddColumn("dbo.Devices", "TransferCronExpression", c => c.String());
            DropColumn("dbo.Devices", "SyncCronExpression");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Devices", "SyncCronExpression", c => c.String());
            DropColumn("dbo.Devices", "TransferCronExpression");
            DropColumn("dbo.Devices", "SyncTimeCronExpression");
        }
    }
}
