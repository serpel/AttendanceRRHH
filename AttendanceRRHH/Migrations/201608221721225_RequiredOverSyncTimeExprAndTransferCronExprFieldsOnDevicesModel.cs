namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredOverSyncTimeExprAndTransferCronExprFieldsOnDevicesModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Devices", "SyncTimeCronExpression", c => c.String(nullable: false));
            AlterColumn("dbo.Devices", "TransferCronExpression", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Devices", "TransferCronExpression", c => c.String());
            AlterColumn("dbo.Devices", "SyncTimeCronExpression", c => c.String());
        }
    }
}
