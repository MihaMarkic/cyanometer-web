using Cyanometer.AirQuality.Services.Abstract;
using Cyanometer.Core.Services.Abstract;
using Flurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cyanometer.AirQuality.Services.Implementation.Specific
{
    public class GiosAirQualityService : AirQualityService, IAirQualityService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Got from https://api.gios.gov.pl/pjp-api/rest/station/findAll - Wrocław - Korzeniowskiego</remarks>
        public const int WroclawStationId = 117;
        public GiosAirQualityService(ILogger<GiosAirQualityService> logger, ICyanoHttpClient client) :
            base(logger, client, "https://api.gios.gov.pl/pjp-api/rest/")
        {
        }
        public string DataSourceInfo => "Voivodship Inspectorates for Environmental Protection";
        public string DataSourceUri => "http://www.gios.gov.pl/en/";
        public async Task<AirQualityData> GetIndexAsync(CancellationToken ct)
        {
            logger.LogInformation("Starting retrieving Wroclaw GIOS data");
            try
            {
                var sensors = await GetSensorIdsAsync(ct);
                AirQualityData data = new AirQualityData();
                var tasks = ImmutableDictionary<ParamId, Task<Measurements>>.Empty;
                foreach (var pair in sensors)
                {
                    Task<Measurements> dataTask = pair.Value.HasValue ? GetDataAsync(pair.Value.Value, ct) : Task.FromResult<Measurements>(null);
                    tasks = tasks.Add(pair.Key, dataTask);
                }
                await Task.WhenAll(tasks.Values);
                data.SO2 = tasks[ParamId.SO2].Result?.Values?.FirstOrDefault()?.Value;
                data.PM10 = tasks[ParamId.PM10].Result?.Values?.FirstOrDefault()?.Value;
                data.O3 = tasks[ParamId.O3].Result?.Values?.FirstOrDefault()?.Value;
                data.NO2 = tasks[ParamId.NO2].Result?.Values?.FirstOrDefault()?.Value;
                data.CO = tasks[ParamId.CO].Result?.Values?.FirstOrDefault()?.Value;
                // for date reference take the one from PM10 sensor reading
                data.Date = tasks[ParamId.PM10].Result.Values.First().Date;
                return data;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed retrieving Wroclaw GIOS data for some reason");
                throw;
            }
        }

        internal async Task<ImmutableDictionary<ParamId, int?>> GetSensorIdsAsync(CancellationToken ct)
        {
            logger.LogInformation($"Getting SensorIds");
            var sensors = await GetSensorsAsync(ct);
            var result = ImmutableDictionary<ParamId, int?>.Empty;
            ImmutableDictionary<int, SensorResult> map = sensors.Where(s => s.Param != null).ToImmutableDictionary(s => s.Param.IdParam, s => s);
            foreach (var typeId in Enum.GetValues(typeof(ParamId)).Cast<ParamId>())
            {
                if (map.TryGetValue((int)typeId, out SensorResult sr))
                {
                    result = result.Add(typeId, sr.Id);
                }
            }
            logger.LogInformation("Getting SensorIds ... done");
            return result;
        }

        public async Task<Measurements> GetDataAsync(int sensorId, CancellationToken ct)
        {
            string url = Url.Combine(baseUrl, $"data/getData/{sensorId}");
            logger.LogInformation($"Getting sensorId for {url}");
            try
            {
                var response = await client.GetAsync(url);
                var result = JsonConvert.DeserializeObject<Measurements>(response);
                logger.LogInformation($"Getting sensorId for {url} ... done");
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed retrieving Wroclaw GIOS raw data");
                return null;
            }
        }

        public async Task<SensorResult[]> GetSensorsAsync(CancellationToken ct)
        {
            var url = Url.Combine(baseUrl, $"station/sensors/{WroclawStationId}");
            logger.LogInformation($"Getting sensors for {url}");
            try
            {
                var response = await client.GetAsync(url);
                var result = JsonConvert.DeserializeObject<SensorResult[]>(response);
                logger.LogInformation($"Getting sensors for {url} ... done");
                return result;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed retrieving Wroclaw GIOS sensors meta data: {response.ErrorMessage}");
                return null;
            }
        }

        public enum ParamId
        {
            SO2 = 1,
            PM10 = 3,
            O3 = 5,
            NO2 = 6,
            CO = 8
        }

        [DebuggerDisplay("{Id}:{Param.ParamCode}")]
        public class SensorResult
        {
            public int Id { get; set; }
            public int StationId { get; set; }
            public SensorParamResult Param { get; set; }
        }

        [DebuggerDisplay("{ParamCode,nq}")]
        public class SensorParamResult
        {
            public string ParamName { get; set; }
            public string ParamFormula { get; set; }
            public string ParamCode { get; set; }
            public int IdParam { get; set; }
        }

        [DebuggerDisplay("{Key,nq}")]
        public class Measurements
        {
            public string Key { get; set; }
            public Measurement[] Values { get; set; }
        }

        [DebuggerDisplay("{Date}")]
        public class Measurement
        {
            public DateTime Date { get; set; }
            public double? Value { get; set; }
        }
    }
}
