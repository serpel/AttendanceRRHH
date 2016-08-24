namespace AttendanceRRHH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDeparmentModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserDepartments",
                c => new
                    {
                        UserDepartmentId = c.Int(nullable: false, identity: true),
                        Id = c.String(nullable: false, maxLength: 128),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserDepartmentId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDepartments", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserDepartments", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.UserDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.UserDepartments", new[] { "Id" });
            DropTable("dbo.UserDepartments");
        }
    }
}
