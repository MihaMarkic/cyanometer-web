using Cyanometer.Core.Services.Abstract;
using Cyanometer.Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Cyanometer.Core.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCyanoCore(this IServiceCollection services)
        {
            return services.AddSingleton<ICyanoHttpClient, CyanoHttpClient>();
        }
    }
}
