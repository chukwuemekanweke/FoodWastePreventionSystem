using Microsoft.Owin;
using Owin;
using Hangfire;
using System;
using FoodWastePreventionSystem.Models;
using FoodWastePreventionSystem.Infrastructure;
using Ninject;
using FoodWastePreventionSystem.App_Start;

[assembly: OwinStartupAttribute(typeof(FoodWastePreventionSystem.Startup))]
namespace FoodWastePreventionSystem
{
    public partial class Startup
    {                  

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var kernel = new StandardKernel();
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

            GlobalConfiguration.Configuration
                .UseSqlServerStorage("AppConnectionString");

            GlobalConfiguration.Configuration.UseNinjectActivator(new Ninject.Web.Common.Bootstrapper().Kernel);

            app.UseHangfireDashboard();
            app.UseHangfireServer();


        }
    }
}
