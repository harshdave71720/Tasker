using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Identity.Application.Services;
using Tasker.Identity.Infrastructure.Configuration;
using Tasker.Identity.Infrastructure.Models;
using Tasker.Identity.Infrastructure.Services;

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
            services.AddIdentityCore<AppIdentityUser>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                }
            );
            services.AddScoped<IUserStore<AppIdentityUser>, UserOnlyStore<AppIdentityUser, AppIdentityDbContext>>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    //ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtConfiguration:Issuer"],
                    //ValidAudience = configuration["JwtConfiguration:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfiguration:Key"]))
                };
            });
            services.Configure<JwtConfigurationOptions>(configuration.GetSection(JwtConfigurationOptions.JwtConfiguration));
            services.AddScoped<IBearerTokenService, BearerTokenService>();
        }
    }
}
