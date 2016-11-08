using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Dashboard;
using AttendanceRRHH.BLL;

[assembly: OwinStartupAttribute(typeof(AttendanceRRHH.Startup))]

namespace AttendanceRRHH
{
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
               .UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard("/jobs");
            app.UseHangfireServer();
            ConfigureAuth(app);
        }
    }
}
