using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using Cyanometer.Core.Services.Abstract;
using Flurl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class SachsenAirQualityService : AirQualityService, IAirQualityService
    {
        readonly IMemoryCache cache;
        public string DataSourceInfo => "Dresden-Winckelmannstraße";
        public string DataSourceUri => "https://geoportal.umwelt.sachsen.de/";
        public SachsenAirQualityService(ILogger<ArsoService> logger, ICyanoHttpClient client, IMemoryCache cache)
            : base(logger, client, "https://geoportal.umwelt.sachsen.de/arcgis/services/luft/luftmessdaten/MapServer/WFSServer")
        {
            this.cache = cache;
        }

        public async Task<AirQualityData> GetIndexAsync(string locationId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                throw new ArgumentNullException(nameof(locationId));
            }
            var result = await cache.GetOrCreateAsync(CacheKeys.SachsenData, async ce =>
            {
                logger.LogInformation("Starting retrieving Dresden data");
                try
                {
                    XDocument doc = await GetDataAsync(ct);
                    ce.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    return ParseData(logger, doc, locationId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed retrieving Dresden data for some reason");
                    throw;
                }
            });
            return result;
        }
        public static DateTime GetDate(string date, string time)
        {
            string properDate = $"{date.Split('T')[0]}T{time}:00";
            return DateTime.Parse(properDate);
        }
        public static AirQualityData ParseData(ILogger logger, XDocument doc, string stationCode)
        {
            var wfs = XNamespace.Get("http://www.opengis.net/wfs/2.0");
            var luftLuftmessdaten = XNamespace.Get("https:geoportal.umwelt.sachsen.de/arcgis/services/luft/luftmessdaten/MapServer/WFSServer");
            var result = doc!.Root
                .Elements().Where(e => e.Name == wfs + "member")
                .Elements().Where(e => e.Name == (luftLuftmessdaten + "Luftmessstationen")
                     && e.Elements().Where(e => e.Name == (luftLuftmessdaten + "EU_CODE") && e.Value == "DESN092").Any())
                .Select(e => new AirQualityData
                {
                    Date = GetDate(e.Element(luftLuftmessdaten + "DATUM").Value, e.Element(luftLuftmessdaten + "MESSZEIT").Value),
                    O3 = GetDoubleValue(logger, e.Element(luftLuftmessdaten + "OZON")),
                    PM10 = GetDoubleValue(logger, e.Element(luftLuftmessdaten + "PM10")),
                    NO2 = GetDoubleValue(logger, e.Element(luftLuftmessdaten + "STICKSTOFFDIOXID")),
                    SO2 = GetDoubleValue(logger, e.Element(luftLuftmessdaten + "SCHWEFELDIOXID")),
                })
                .FirstOrDefault();
            return result;
        }

        public async Task<XDocument> GetDataAsync(CancellationToken ct)
        {
            const string parameters = "?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=luft_luftmessdaten:Luftmessstationen&TYPENAME=luft_luftmessdaten:Luftmessstationen&STARTINDEX=0&COUNT=1000&SRSNAME=urn:ogc:def:crs:EPSG::25833&BBOX=5431070.18768121488392353,215209.8988020783290267,5848104.34905883856117725,564673.96820965665392578,urn:ogc:def:crs:EPSG::25833";
            string content = await client.GetAsync(Url.Combine(baseUrl, parameters));
            XDocument doc = XDocument.Parse(content);
            return doc;
        }
    }
}
