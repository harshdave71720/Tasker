﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Application.Repositories;
using Tasker.Infrastructure.Repositories;
using Tasker.Infrastructure.Settings;

namespace Tasker.Infrastructure
{
    public static class RegisterServices
    {
        public static void ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataStoreSettings>(configuration.GetSection(DataStoreSettings.SectionName));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<CoreObjectConvertor>();
        }
    }
}
