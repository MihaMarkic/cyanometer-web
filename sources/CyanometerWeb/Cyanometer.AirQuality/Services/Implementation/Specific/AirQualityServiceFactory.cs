using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using System;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class AirQualityServiceFactory
    {
        readonly ArsoService arsoService;
        readonly GiosAirQualityService giosService;
        readonly SabraAirQualityService sabraService;
        public AirQualityServiceFactory(ArsoService arsoService, GiosAirQualityService giosService, SabraAirQualityService sabraService)
        {
            this.arsoService = arsoService;
            this.giosService = giosService;
            this.sabraService = sabraService;
        }
        public IAirQualityService GetService(AirQualitySource source) =>
            source switch
            {
                AirQualitySource.Gios => giosService,
                AirQualitySource.Arso => arsoService,
                AirQualitySource.Sabra => sabraService,
                _ => throw new Exception($"Unsupported air quality source {source}")
            };
    }
}
