using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.AirQualityTest.Services.Implementation;
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
        readonly SachsenAirQualityService sachsenService;
        readonly UmweltKtnGvAtAirQualityService umweltKtnGvAtService;
        public AirQualityServiceFactory(ArsoService arsoService, GiosAirQualityService giosService, 
            SabraAirQualityService sabraService, AqicnAirQualityService aqicnService, SachsenAirQualityService sachsenService,
            UmweltKtnGvAtAirQualityService umweltKtnGvAtService)
        {
            this.arsoService = arsoService;
            this.giosService = giosService;
            this.sabraService = sabraService;
            this.aqicnService = aqicnService;
            this.sachsenService = sachsenService;
            this.umweltKtnGvAtService = umweltKtnGvAtService;
        }
        public IAirQualityService GetService(AirQualitySource source) =>
            source switch
            {
                AirQualitySource.Gios => giosService,
                AirQualitySource.Arso => arsoService,
                AirQualitySource.Sabra => sabraService,
                AirQualitySource.Aqicn => aqicnService,
                AirQualitySource.Sachsen => sachsenService,
                AirQualitySource.UmweltKtnGvAt => umweltKtnGvAtService,
                _ => throw new Exception($"Unsupported air quality source {source}")
            };
    }
}
