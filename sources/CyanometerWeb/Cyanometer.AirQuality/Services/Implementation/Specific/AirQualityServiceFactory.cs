﻿using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using System;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class AirQualityServiceFactory
    {
        readonly ArsoService arsoService;
        readonly GiosAirQualityService giosService;
        public AirQualityServiceFactory(ArsoService arsoService, GiosAirQualityService giosService)
        {
            this.arsoService = arsoService;
            this.giosService = giosService;
        }
        public IAirQualityService GetService(AirQualitySource source) =>
            source switch
            {
                AirQualitySource.Gios => giosService,
                AirQualitySource.Arso => arsoService,
                _ => throw new Exception($"Unsupported air quality source {source}")
            };
    }
}
