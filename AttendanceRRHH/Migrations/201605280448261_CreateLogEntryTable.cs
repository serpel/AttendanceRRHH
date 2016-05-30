namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateLogEntryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        Username = c.String(),
                        Level = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                        CallSite = c.String(),
                        Logger = c.String(),
                        StackTrace = c.String(),
                        Thread = c.String(),
                        MachineName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogEntries");
        }
    }
}
