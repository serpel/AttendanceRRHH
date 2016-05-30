namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedLogEntryTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogEntries", "Application", c => c.String());
            AddColumn("dbo.LogEntries", "ServerName", c => c.String());
            AddColumn("dbo.LogEntries", "Port", c => c.String());
            AddColumn("dbo.LogEntries", "Url", c => c.String());
            AddColumn("dbo.LogEntries", "RemoteAddress", c => c.String());
            AlterColumn("dbo.LogEntries", "Date", c => c.DateTime(nullable: false));
            DropColumn("dbo.LogEntries", "StackTrace");
            DropColumn("dbo.LogEntries", "Thread");
            DropColumn("dbo.LogEntries", "MachineName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LogEntries", "MachineName", c => c.String());
            AddColumn("dbo.LogEntries", "Thread", c => c.String());
            AddColumn("dbo.LogEntries", "StackTrace", c => c.String());
            AlterColumn("dbo.LogEntries", "Date", c => c.String());
            DropColumn("dbo.LogEntries", "RemoteAddress");
            DropColumn("dbo.LogEntries", "Url");
            DropColumn("dbo.LogEntries", "Port");
            DropColumn("dbo.LogEntries", "ServerName");
            DropColumn("dbo.LogEntries", "Application");
        }
    }
}
