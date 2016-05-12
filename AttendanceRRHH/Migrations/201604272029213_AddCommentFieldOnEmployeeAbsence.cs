namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentFieldOnEmployeeAbsence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeAbsences", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeAbsences", "Comment");
        }
    }
}
