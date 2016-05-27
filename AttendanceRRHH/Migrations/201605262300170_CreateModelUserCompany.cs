namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModelUserCompany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCompanies",
                c => new
                    {
                        UserCompanyId = c.Int(nullable: false, identity: true),
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserCompanyId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCompanies", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCompanies", "CompanyId", "dbo.Companies");
            DropIndex("dbo.UserCompanies", new[] { "CompanyId" });
            DropIndex("dbo.UserCompanies", new[] { "Id" });
            DropTable("dbo.UserCompanies");
        }
    }
}
