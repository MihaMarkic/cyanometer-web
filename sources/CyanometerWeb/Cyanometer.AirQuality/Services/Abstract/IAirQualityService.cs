using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cyanometer.AirQuality.Services.Abstract
{
    public interface IAirQualityService
    {
        string DataSourceInfo { get; }
        string DataSourceUri { get; }
        Task<AirQualityData> GetIndexAsync(CancellationToken ct);
    }
}