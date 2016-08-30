namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyIdFieldToJobPositionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobPositions", "CompanyId", c => c.Int());
            CreateIndex("dbo.JobPositions", "CompanyId");
            AddForeignKey("dbo.JobPositions", "CompanyId", "dbo.Companies", "CompanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobPositions", "CompanyId", "dbo.Companies");
            DropIndex("dbo.JobPositions", new[] { "CompanyId" });
            DropColumn("dbo.JobPositions", "CompanyId");
        }
    }
}
