using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Tasker.Core.TaskWorkerOrdering.Factories;
using Tasker.Core.Factories;

namespace Tasker.Application
{
    public static class RegisterServices
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IWorkerOrdererFactory, WorkerOrdererFactory>();
            services.AddSingleton<ITaskFactory, TaskFactory>();
        }
    }
}
