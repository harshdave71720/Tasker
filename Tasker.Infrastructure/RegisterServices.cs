using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Infrastructure.Settings;

namespace Tasker.Infrastructure
{
    public static class RegisterServices
    {
        public static void ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataStoreSettings>(configuration.GetSection(DataStoreSettings.SectionName));
        }
    }
}
