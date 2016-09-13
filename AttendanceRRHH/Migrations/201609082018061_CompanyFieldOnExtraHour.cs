namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyFieldOnExtraHour : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ExtraHours", "CompanyId");
            AddForeignKey("dbo.ExtraHours", "CompanyId", "dbo.Companies", "CompanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExtraHours", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ExtraHours", new[] { "CompanyId" });
        }
    }
}
