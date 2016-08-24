namespace AttendanceRRHH.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AttendanceRRHH.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AttendanceRRHH.Models.ApplicationDbContext context)
        {
            string email = "sergio.peralta@kattangroup.com";

            //create the first user
            if (!(context.Users.Any(u => u.Email == email)))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser { UserName = email, Email = email };
                var result = userManager.Create(userToInsert, "Admin.1234");

                //create and asign roles
                if (result.Succeeded)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    var adminRole = new IdentityRole("Admin");
                    result = roleManager.Create(adminRole);

                    if (result.Succeeded)
                    {
                        userManager.AddToRoles(userToInsert.Id, adminRole.Name);
                    }                   
                }
            }

            context.Absences.AddOrUpdate(x => x.AbsenceId,
               new Absence() { AbsenceId = 1, Name = "Vacaciones", IsActive = true },
               new Absence() { AbsenceId = 2, Name = "Feriado", IsActive = true },
               new Absence() { AbsenceId = 3, Name = "Enfermedad", IsActive = true },
               new Absence() { AbsenceId = 4, Name = "Ausencia de Marca", IsActive = true },
               new Absence() { AbsenceId = 5, Name = "Otros", IsActive = true }
               );

            context.Jobs.AddOrUpdate(x => x.JobPositionId,
                 new JobPosition() { JobPositionId = 1, JobTitle = "Otros", IsActive = true }
             );

            context.Countries.AddOrUpdate(x => x.CountryId,
                new Country() { CountryId = 1, Name = "Honduras", IsActive = true },
                new Country() { CountryId = 2, Name = "Guatemala", IsActive = true },
                new Country() { CountryId = 3, Name = "Costa Rica", IsActive = true }
                );

            context.Cities.AddOrUpdate(x => x.CityId,
                new City() { CityId = 1, CountryId = 1, Name = "Choloma", IsActive = true },
                new City() { CityId = 2, CountryId = 1, Name = "Puerto Cortes", IsActive = true },
                new City() { CityId = 3, CountryId = 1, Name = "San Pedro Sula", IsActive = true }
                );

            context.DeviceTypes.AddOrUpdate(x => x.DeviceTypeId,
                new DeviceType() { DeviceTypeId = 1, Name = "ZK", Description = "ZKTecno Device", IsActive = true }
                );

            Console.WriteLine("Finish");
            context.SaveChanges();
        }
    }
}
