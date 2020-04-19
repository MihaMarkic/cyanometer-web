namespace Cyanometer.AirQuality.Services.Abstract
{
    public interface IAirQualityProcessor
    {
        AirQualityPollutionCalculated Calculate(AirQualityData data);
    }
}
