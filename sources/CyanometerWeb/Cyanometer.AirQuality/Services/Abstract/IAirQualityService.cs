using System.Threading;
using System.Threading.Tasks;

namespace Cyanometer.AirQuality.Services.Abstract
{
    public interface IAirQualityService
    {
        string DataSourceInfo { get; }
        string DataSourceUri { get; }
        Task<AirQualityData> GetIndexAsync(string locationId, CancellationToken ct);
    }
}