using Cyanometer.AirQuality;
using Cyanometer.AirQuality.Services.Implementation;
using Cyanometer.AirQuality.Services.Implementation.Specific;
using Cyanometer.Core;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cyanometer.Web.Pages
{
    public class LocationModel : PageModel
    {
        readonly ILogger<LocationModel> logger;
        readonly AirQualityServiceFactory airQualityServiceFactory;
        readonly AirQualityProcessor airQualityProcessor;
        readonly IMemoryCache memoryCache;
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AirQualitySource { get; private set; }
        public string AirQualityLink { get; private set; }
        public string AirQualityHost => new Uri(AirQualityLink).Host;
        public string PollutionMeasurement { get; private set; }
        public string PollutionColor { get; private set; }
        public string PollutionText { get; private set; }
        public string LevelsText { get; private set; }
        public Pollution Pollution { get; private set; }
        public LocationModel(ILogger<LocationModel> logger, IMemoryCache memoryCache, 
            AirQualityServiceFactory airQualityServiceFactory, AirQualityProcessor airQualityProcessor)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.airQualityServiceFactory = airQualityServiceFactory;
            this.airQualityProcessor = airQualityProcessor;
        }
        public async Task<IActionResult> OnGetAsync(string city, string country)
        {
            City = city.Titleize();
            Country = country.Titleize();
            var cyanometerDataSource = CyanometerDataSources.Default.GetData(city, country);
            if (cyanometerDataSource == null)
            {
                logger.LogWarning($"Couldn't find source {country}/{city}");
                return NotFound();
            }
            string key = $"air_pollution_for_{country}/{city}";
            var service = airQualityServiceFactory.GetService(cyanometerDataSource.AirQualitySource);
            AirQualitySource = service.DataSourceInfo;
            AirQualityLink = service.DataSourceUri;
            AirQualityPollutionCalculated? calculated = await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                try
                {
                    var rawPollution = await service.GetIndexAsync(cyanometerDataSource.AirQualityLocation, CancellationToken.None);
                    calculated = airQualityProcessor.Calculate(rawPollution);
                    entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(15));
                    return calculated;
                }
                catch (Exception)
                {
                    entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(1));
                    return null;
                }
            });
            if (calculated is not null)
            {
                PollutionMeasurement = calculated.PollutionWeight != AirPollution.Low ? calculated.ChiefPollutant.ToString() : "LOW";
                PollutionColor = calculated.PollutionWeight switch
                {
                    AirPollution.Low => "72B22F",
                    AirPollution.Mid => "FEF10D",
                    AirPollution.High => "F17E19",
                    AirPollution.VeryHigh => "E4001C",
                    _ => ""
                };
                PollutionText = calculated.PollutionWeight switch
                {
                    AirPollution.Low => "LOW",
                    AirPollution.Mid => "MODERATE",
                    AirPollution.High => "HIGH",
                    AirPollution.VeryHigh => "VERY HIGH",
                    _ => "UNKNOWN"
                };
                LevelsText = calculated.ChiefPollutant switch
                {
                    Measurement.NO2 => "NITROGEN DIOXIDE",
                    Measurement.SO2 => "SULFUR DIOXIDE",
                    Measurement.O3 => "OZONE",
                    Measurement.PM10 => "PARTICLES",
                    _ => ""
                };
                Pollution = new Pollution(
                    FromDouble(calculated.Data.O3),
                    FromDouble(calculated.Data.PM10),
                    FromDouble(calculated.Data.SO2),
                    FromDouble(calculated.Data.NO2)
                );
            }
            else
            {
                Pollution = new Pollution(null, null, null, null);
            }
            return Page();
        }

        int? FromDouble(double? value) => value.HasValue ? (int?)Convert.ToInt32(value.Value): null;
    }

    public class Pollution
    {
        public int? Ozone { get; }
        public int? PM10 { get; }
        public int? SO2 { get; }
        public int? NO2 { get; }
        public Pollution(int? ozone, int? pM10, int? sO2, int? nO2)
        {
            Ozone = ozone;
            PM10 = pM10;
            SO2 = sO2;
            NO2 = nO2;
        }
    }
}