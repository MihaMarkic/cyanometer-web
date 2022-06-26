using Cyanometer.AirQuality;
using System.Diagnostics.CodeAnalysis;

namespace Cyanometer.AirQualityTest.Services.Implementation;

public partial class SabraAirQualityServiceTest
{
    public class AirQualityDataEqualityComparer : IEqualityComparer<AirQualityData>
    {
        public static AirQualityDataEqualityComparer Default = new AirQualityDataEqualityComparer();
        public bool Equals(AirQualityData? x, AirQualityData? y)
        {
            if (x is null && y is null)
            {
                return true;
            }
            if (x is null ^ y is null)
            {
                return false;
            }
            return x!.Date == y!.Date && x.NO2 == y.NO2 && x.O3 == y.O3 && x.PM10 == y.PM10 && x.SO2 == y.SO2;
        }

        public int GetHashCode([DisallowNull] AirQualityData obj)
        {
            return HashCode.Combine(obj.Date, obj.NO2, obj.O3, obj.PM10, obj.SO2);
        }
    }
}
