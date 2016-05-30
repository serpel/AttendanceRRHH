namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedApplicationFieldFromLogEntry : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LogEntries", "Application");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LogEntries", "Application", c => c.String());
        }
    }
}
