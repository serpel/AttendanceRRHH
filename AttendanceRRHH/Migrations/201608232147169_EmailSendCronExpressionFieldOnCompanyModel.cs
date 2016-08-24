namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailSendCronExpressionFieldOnCompanyModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "EmailSendCronExpression", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "EmailSendCronExpression");
        }
    }
}
