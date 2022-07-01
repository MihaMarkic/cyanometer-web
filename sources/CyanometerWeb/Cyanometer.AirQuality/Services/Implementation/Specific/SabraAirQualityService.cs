using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using FluentFTP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cyanometer.AirQuality.Services.Implementation.Specific;

public class SabraAirQualityService : IAirQualityService
{
    readonly IMemoryCache cache;
    readonly ILogger logger;
    readonly FtpClient ftpClient;
    const string host = "ftp://fnwi.vps.infomaniak.com";
    const string remoteFile = "data/Necker_xml";
    public string DataSourceInfo => "Etat de Genève – SABRA";
    public string DataSourceUri => "https://www.ge.ch/connaitre-qualite-air-geneve/dernier-bulletin-qualite-air-geneve";
    public SabraAirQualityService(ILogger<SabraAirQualityService> logger, IMemoryCache cache)
    {
        this.logger = logger;
        this.cache = cache;
        ftpClient = new FtpClient(host, SabraAirQualityCredentials.Instance.Username, SabraAirQualityCredentials.Instance.Password);
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
                return ParseData(logger, doc, locationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed retrieving data for some reason");
                throw;
            }
        });
        return result;
    }

    public static AirQualityData ParseData(ILogger logger, XDocument doc, string stationCode)
    {
        var root = doc.Root.Element("messstation").Element("messstellengruppe").Element("messstelle");
        return new AirQualityData
        {
            Date = GetDate(logger, doc.Root),
            NO2 = GetValueFor(logger, root, "NO2"),
            O3 = GetValueFor(logger, root, "O3"),
            PM10 = GetValueFor(logger, root, "PM10"),
            SO2 = null,
        };
    }

    public static double? GetValueFor(ILogger logger, XElement root, string groupType)
    {
        try
        {
            var channel = root.Elements()
                .Where(e => string.Equals((string)e.Attribute("kanalgruppetyp"), groupType, StringComparison.Ordinal))
                .Single()
                .Element("kanal");
            var lastRecord = channel.Elements().Where(e => e.Attribute("wert") is not null).Last();
            return double.Parse((string)lastRecord.Attribute("wert"), CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Couldn't parse value {root.Value} for code {groupType}");
            return null;
        }
    }

    public static DateTime GetDate(ILogger logger, XElement root)
    {
        return DateTime.Parse((string)root.Attribute("ende"), new CultureInfo("de_DE"));
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
