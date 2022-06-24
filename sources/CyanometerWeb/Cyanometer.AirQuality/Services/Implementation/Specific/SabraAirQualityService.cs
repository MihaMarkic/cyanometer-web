using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using FluentFTP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class SabraAirQualityService : IAirQualityService
    {
        readonly IMemoryCache cache;
        readonly ILogger logger;
        readonly FtpClient ftpClient;
        const string host = "ftp://fnwi.vps.infomaniak.com";
        const string remoteFile = "data/Necker_xml";
        const string user = "";
        const string password = "";
        public string DataSourceInfo => "Etat de Genève – SABRA";
        public string DataSourceUri => "https://www.ge.ch/connaitre-qualite-air-geneve/dernier-bulletin-qualite-air-geneve";
        public SabraAirQualityService(ILogger<SabraAirQualityService> logger, IMemoryCache cache)
        {
            this.logger = logger;
            this.cache = cache;
            ftpClient = new FtpClient(host, user, password);
        }

        public async Task<AirQualityData> GetIndexAsync(string locationId, CancellationToken ct)
        {
            var result = await cache.GetOrCreateAsync(CacheKeys.SabreData, async ce =>
            {
                logger.LogInformation("Starting retrieving SABRE data");
                try
                {
                    XDocument doc = await GetDataAsync(ct);
                    ce.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    return ParseData(doc, locationId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed retrieving data for some reason");
                    throw;
                }
            });
            return result;
        }

        public AirQualityData ParseData(XDocument doc, string stationCode)
        {
            var root = doc.Root.Element("messstation").Element("messstellengruppe").Element("messstelle");
            return new AirQualityData
            {
                NO2 = GetValueFor(root, "NO2"),
                O3 = GetValueFor(root, "O3"),
                PM10 = GetValueFor(root, "PM10"),
                SO2 = null,
            };
        }

        public double? GetValueFor(XElement root, string groupType)
        {
            try
            {
                var channel = root.Elements()
                    .Where(e => string.Equals((string)e.Attribute("kanalgruppetyp"), groupType, StringComparison.Ordinal))
                    .Single()
                    .Element("kanal");
                var lastRecord = channel.Elements().Last();
                return double.Parse((string)lastRecord.Attribute("wert"), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Couldn't parse value {root.Value} for code {groupType}");
                return null;
            }
        }

        public async Task<XDocument> GetDataAsync(CancellationToken ct)
        {
            await ftpClient.ConnectAsync(ct);
            try
            {
                var memoryStream = new MemoryStream();
                await ftpClient.DownloadAsync(memoryStream, remoteFile, token: ct);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memoryStream))
                {
                    var text = reader.ReadToEnd();
                    XDocument doc = XDocument.Parse(text);
                    return doc;
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed retrieving data");
                throw;
            }
            finally
            {
                await ftpClient.DisconnectAsync(ct);
            }
        }
    }
}
