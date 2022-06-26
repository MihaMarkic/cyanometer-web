using System;
using System.Threading;
using System.Threading.Tasks;
using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core;
using Cyanometer.Core.Services.Abstract;
using Flurl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cyanometer.AirQuality.Services.Implementation.Specific;

public class AqicnAirQualityService : AirQualityService, IAirQualityService
{
    readonly IMemoryCache cache;
    public string DataSourceInfo => "World's Air Pollution";
    public string DataSourceUri => "https://waqi.info/";
    readonly string url;
    public AqicnAirQualityService(ILogger<ArsoService> logger, ICyanoHttpClient client, IMemoryCache cache)
            : base(logger, client, "https://api.waqi.info/feed/")
    {
        this.cache = cache;
        url = $"dresden/?token={AqicnCredentials.Instance.Token}";
    }

    public async Task<AirQualityData> GetIndexAsync(string locationId, CancellationToken ct)
    {
        var result = await cache.GetOrCreateAsync(CacheKeys.SabreData, async ce =>
        {
            logger.LogInformation("Starting retrieving SABRE data");
            try
            {
                AqicnResult data = await GetDataAsync(ct);
                ce.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return ParseData(logger, data, locationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed retrieving data for some reason");
                throw;
            }
        });
        return result;
    }

    public static AirQualityData ParseData(ILogger logger, AqicnResult data, string stationCode)
    {
        return new AirQualityData
        {
            Date = data.Data.Time.Iso.DateTime,
            NO2 = data.Data.Iaqi.No2?.V,
            O3 = data.Data.Iaqi.O3?.V,
            PM10 = data.Data.Iaqi.Pm10?.V,
            SO2 = data.Data.Iaqi.So2?.V,
        };
    }

    public async Task<AqicnResult> GetDataAsync(CancellationToken ct)
    {
        string content = await client.GetAsync(Url.Combine(baseUrl, url));
        var result = DeserializeData(content);
        return result;
    }

    public static AqicnResult DeserializeData(string content) => JsonConvert.DeserializeObject<AqicnResult>(content);

    public record AqicnResult(string? Status, AqicnData Data);
    public record AqicnData(string Aqi, AqicnIaqi Iaqi, AqicnTime Time);
    public record AqicnIaqi(AqicnValue No2, AqicnValue O3, AqicnValue Pm10, AqicnValue So2);
    public record AqicnValue(double? V);
    public record AqicnTime(DateTimeOffset Iso);
}
