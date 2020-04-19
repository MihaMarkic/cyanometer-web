using Cyanometer.AirQuality.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;

namespace Cyanometer.AirQuality.Services.Implementation
{
    public class AirQualityProcessor : IAirQualityProcessor
    {
        protected readonly ILogger<AirQualityProcessor> logger;

        public AirQualityProcessor(ILogger<AirQualityProcessor> logger)
        {
            this.logger = logger;
        }

        public AirQualityPollutionCalculated Calculate(AirQualityData data)
        {
            var pollutions = CalculatePollution(data);
            var calculatedPollution = CalculateMaxPollution(pollutions);
            AirPollution pollution = calculatedPollution.Pollution;
            Measurement chief = GetChiefPolluter(pollutions);
            string pollutionInfo = $"Max pollution is {pollution} with index {calculatedPollution.Index:0} coming from {chief}";
            logger.LogInformation(pollutionInfo);
            logger.LogInformation("Air quality processor done");
            return new AirQualityPollutionCalculated(data, pollution, chief);
        }

        public CalculatedPollution[] CalculatePollution(AirQualityData data)
        {
            logger.LogInformation("Calculating pollution");
            CalculatedPollution[] pollution = new CalculatedPollution[]
            {
                CalculatePollution(data.Date, Measurement.PM10, data.PM10, CalculatePM10PollutionIndex(data.PM10 ?? 0)),
                CalculatePollution(data.Date, Measurement.O3, data.O3, CalculateO3PollutionIndex(data.O3 ?? 0)),
                CalculatePollution(data.Date, Measurement.NO2, data.NO2, CalculateNO2PollutionIndex(data.NO2 ?? 0)),
                CalculatePollution(data.Date, Measurement.SO2, data.SO2, CalculateSO2PollutionIndex(data.SO2 ?? 0))
            };
            return pollution;
        }

        public static CalculatedPollution CalculateMaxPollution(CalculatedPollution[] pollutions)
        {
            CalculatedPollution result = null;
            foreach (var pollution in pollutions)
            {
                if (result == null || pollution.Index > result.Index)
                {
                    result = pollution;
                }
            }
            return result;
        }

        public static Measurement GetChiefPolluter(CalculatedPollution[] pollutions)
        {
            Measurement measurement = Measurement.NO2;
            double maxIndex = double.MinValue;
            foreach (var pollution in pollutions)
            {
                if (pollution.Index > maxIndex)
                {
                    measurement = pollution.Measurement;
                    maxIndex = pollution.Index;
                }
            }
            return measurement;
        }

        public CalculatedPollution CalculatePollution(DateTime measurementDate, Measurement measurement, double? value, double? index)
        {
            CalculatedPollution result = new CalculatedPollution { Measurement = measurement, Index = value ?? 0 };
            bool isOutdated = false;
            if (!value.HasValue || (isOutdated = (DateTime.Now - measurementDate).TotalHours > 4))
            {
                result.Pollution = AirPollution.Low;
                result.Index = 0;
            }
            else
            {
                if (index.Value < 50)
                {
                    result.Pollution = AirPollution.Low;
                }
                else if (index < 75)
                {
                    result.Pollution = AirPollution.Mid;
                }
                else if (index < 100)
                {
                    result.Pollution = AirPollution.High;
                }
                else
                {
                    result.Pollution = AirPollution.VeryHigh;
                }
                result.Index = index.Value;
            }
            string outdatedText = isOutdated ? "OUTDATED " : "";
            logger.LogInformation($"{outdatedText}{measurement} is {result.Pollution} ({value} with index {result.Index:0.00}) on {measurementDate}");
            return result;
        }

        public static double CalculatePM10PollutionIndex(double c)
        {
            if (c < 30)
            {
                return c * 50.0 / 30.0;
            }
            else if (c < 50)
            {
                return 50.0 + 25.0 / 20.0 * (c - 30);
            }
            else if (c < 100)
            {
                return 75 + 25.0 / 50.0 * (c - 50);
            }
            else
            {
                return c;
            }
        }

        public static double CalculateO3PollutionIndex(double c)
        {
            if (c < 60)
            {
                return c * 50.0 / 60.0;
            }
            else if (c < 120)
            {
                return 50 + 25.0 / 60.0 * (c - 60);
            }
            else if (c < 180)
            {
                return 75 + 25.0 / 60.0 * (c - 120);
            }
            else
            {
                return c * 100.0 / 180.0;
            }
        }

        public static double CalculateNO2PollutionIndex(double c)
        {
            if (c < 50)
            {
                return c;
            }
            else if (c < 100)
            {
                return 50 + 25.0 / 50.0 * (c - 50);
            }
            else if (c < 200)
            {
                return 50 + 25.0 / 100.0 * (c - 100);
            }
            else
            {
                return c * 100.0 / 200.0;
            }
        }

        public static double CalculateSO2PollutionIndex(double c)
        {
            if (c < 50)
            {
                return c;
            }
            else if (c < 100)
            {
                return 50 + 25.0 / 50.0 * (c - 50);
            }
            else if (c < 350)
            {
                return 75 + 25.0 / 250.0 * (c - 100);
            }
            else
            {
                return c * 100.0 / 350.0;
            }
        }
    }
}
