using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartupAttribute(typeof(FoodWastePreventionSystem.Startup))]
namespace FoodWastePreventionSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            GlobalConfiguration.Configuration
                .UseSqlServerStorage("AppConnectionString");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
