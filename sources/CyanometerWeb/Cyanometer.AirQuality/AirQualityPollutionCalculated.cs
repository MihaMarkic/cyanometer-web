namespace Cyanometer.AirQuality
{
    public class AirQualityPollutionCalculated
    {
        public AirQualityData Data { get; }
        public AirPollution PollutionWeight { get; }
        public Measurement ChiefPollutant { get; }
        public AirQualityPollutionCalculated(AirQualityData data, AirPollution pollutionWeight, Measurement chiefPollutant)
        {
            Data = data;
            PollutionWeight = pollutionWeight;
            ChiefPollutant = chiefPollutant;
        }
    }
}
