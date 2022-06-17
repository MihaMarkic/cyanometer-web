using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using Cyanometer.Core.Services.Abstract;
using Flurl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class ArsoService : AirQualityService, IAirQualityService
    {
        private const string url = "ones_zrak_urni_podatki_zadnji.xml";
        readonly IMemoryCache cache;
        public ArsoService(ILogger<ArsoService> logger, ICyanoHttpClient client, IMemoryCache cache)
            : base(logger, client, "http://www.arso.gov.si/xml/zrak/")
        {
            this.cache = cache;
        }
        public string DataSourceInfo => "ARSO Ljubljana Bežigrad";
        public string DataSourceUri => "https://www.arso.gov.si/";
        public async Task<AirQualityData> GetIndexAsync(string locationId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                throw new ArgumentNullException(nameof(locationId));
            }
            var result = await cache.GetOrCreateAsync(CacheKeys.ArsoData, async ce =>
            {
                logger.LogInformation("Starting retrieving arso data");
                try
                {
                    XDocument doc = await GetDataAsync(ct);
                    ce.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    return ParseData(doc, locationId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed retrieving arso data for some reason");
                    throw;
                }
            });
            return result;
        }

        public AirQualityData ParseData(XDocument doc, string stationCode)
        {
            XElement root = doc.Root;
            if (!string.Equals(root.Name.LocalName, "arsopodatki", StringComparison.Ordinal))
            {
                logger.LogError($"Root node name is {root.Name.LocalName} while expected arsopodatki");
                return null;
            }
            AirQualityData result = new AirQualityData
            {
                Date = DateTime.Parse(root.Element("datum_priprave").Value, CultureInfo.InvariantCulture)
            };

            var query = from e in root.Elements("postaja")
                        let code = e.Attribute("sifra")
                        where code != null && code.Value == stationCode
                        select e;
            var station = query.SingleOrDefault();
            if (station == null)
            {
                logger.LogError($"Couldn't find station with sifra={stationCode}");
                return null;
            }
            result.SO2 = GetDoubleValue(station.Element("so2"));
            result.PM10 = GetDoubleValue(station.Element("pm10"));
            result.O3 = GetDoubleValue(station.Element("o3"));
            result.NO2 = GetDoubleValue(station.Element("no2"));
            result.CO = GetDoubleValue(station.Element("co"));
            return result;
        }

        public async Task<XDocument> GetDataAsync(CancellationToken ct)
        {
            string content = await client.GetAsync(Url.Combine(baseUrl, url));
            XDocument doc = XDocument.Parse(content);
            return doc;
        }
    }
}
