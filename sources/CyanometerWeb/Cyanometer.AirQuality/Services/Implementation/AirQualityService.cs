using Cyanometer.Core.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace Cyanometer.AirQuality.Services.Implementation
{
    public abstract class AirQualityService
    {
        protected readonly ICyanoHttpClient client;
        protected readonly ILogger logger;
        protected readonly string baseUrl;
        public AirQualityService(ILogger logger, ICyanoHttpClient client, string baseUrl)
        {
            this.logger = logger;
            this.client = client;
            this.baseUrl = baseUrl;
        }

        public double? GetDoubleValue(XElement element) => GetDoubleValue(logger, element);
        public static double? GetDoubleValue(ILogger logger, XElement element)
        {
            try
            {
                if (element?.Value == null || string.IsNullOrEmpty(element.Value))
                {
                    return null;
                }
                // special case for ARSO
                if (element.Value.StartsWith("<", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                }
                return double.Parse(element.Value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Couldn't parse value {element.Value} for code {element.Name}");
                return null;
            }
        }
    }
}
