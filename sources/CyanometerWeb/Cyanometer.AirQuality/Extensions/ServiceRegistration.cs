﻿using Cyanometer.AirQuality.Services.Implementation;
using Cyanometer.AirQuality.Services.Implementation.Specific;
using Cyanometer.AirQualityTest.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Cyanometer.AirQuality.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAirQuality(this IServiceCollection services)
        {
            return services
                .AddSingleton<AirQualityServiceFactory>()
                .AddSingleton<ArsoService>()
                .AddSingleton<GiosAirQualityService>()
                .AddSingleton<SabraAirQualityService>()
                .AddSingleton<AqicnAirQualityService>()
                .AddSingleton<SachsenAirQualityService>()
                .AddSingleton<UmweltKtnGvAtAirQualityService>()
                .AddSingleton<AirQualityProcessor>();
        }
    }
}
