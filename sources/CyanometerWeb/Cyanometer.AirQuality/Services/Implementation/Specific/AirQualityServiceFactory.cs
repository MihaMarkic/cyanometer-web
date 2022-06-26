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
        readonly AqicnAirQualityService aqicnService;
        public AirQualityServiceFactory(ArsoService arsoService, GiosAirQualityService giosService, 
            SabraAirQualityService sabraService, AqicnAirQualityService aqicnService)
        {
            this.arsoService = arsoService;
            this.giosService = giosService;
            this.sabraService = sabraService;
            this.aqicnService = aqicnService;
        }
        public IAirQualityService GetService(AirQualitySource source) =>
            source switch
            {
                AirQualitySource.Gios => giosService,
                AirQualitySource.Arso => arsoService,
                AirQualitySource.Sabra => sabraService,
                AirQualitySource.Aqicn => aqicnService,
                _ => throw new Exception($"Unsupported air quality source {source}")
            };
    }
}
