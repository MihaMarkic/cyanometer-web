using System;
using System.Collections.Immutable;

namespace Cyanometer.Core
{
    public class CyanometerDataSources
    {
        public const string Wroclaw = "poland/wroclaw";
        public const string Ljubljana = "slovenia/ljubljana";
        public static CyanometerDataSources Default { get; } = new CyanometerDataSources();
        public ImmutableDictionary<string, CyanometerDataSource> Data { get; }
        CyanometerDataSources()
        {
            Data = ImmutableDictionary.Create<string, CyanometerDataSource>(StringComparer.OrdinalIgnoreCase)
                .Add(Ljubljana, new CyanometerDataSource(AirQualitySource.Arso, airQualityLocation: "E21"))
                .Add(Wroclaw, new CyanometerDataSource(AirQualitySource.Gios, airQualityLocation: null));
        }
        public CyanometerDataSource GetData(string city, string country)
        {
            if (Data.TryGetValue($"{country}/{city}", out var cyanometerDataSource))
            {
                return cyanometerDataSource;
            }
            return null;
        }
    }

    public class CyanometerDataSource
    {
        public AirQualitySource Source { get; }
        public string AirQualityLocation { get; }
        public CyanometerDataSource(AirQualitySource source, string airQualityLocation)
        {
            Source = source;
            AirQualityLocation = airQualityLocation;
        }
    }
}
