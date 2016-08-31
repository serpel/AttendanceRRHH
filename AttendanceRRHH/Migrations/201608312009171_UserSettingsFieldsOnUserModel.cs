namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSettingsFieldsOnUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ProfileUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "Culture", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Culture");
            DropColumn("dbo.AspNetUsers", "ProfileUrl");
        }
    }
}
