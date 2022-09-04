using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Infrastructure.Models;

namespace Tasker.Identity.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppIdentityDbContext>(options => 
            {
                options.UseMySql(configuration.GetConnectionString("IdentityDb"),
                    options =>
                    {
                        options.ServerVersion(new Version(8, 0, 25).ToString());
                        var migrationAssembly = typeof(ServiceRegistration).Assembly.GetName().Name;
                        options.MigrationsAssembly(migrationAssembly);
                    });
            });
            services.AddIdentityCore<AppIdentityUser>(options => { });
            services.AddScoped<IUserStore<AppIdentityUser>, UserOnlyStore<AppIdentityUser, AppIdentityDbContext>>();
        }
    }
}
