using System;
using System.Collections.Immutable;
using System.IO;

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
                .Add(Ljubljana, new CyanometerDataSource("Slovenia", "Ljubljana", AirQualitySource.Arso, Ljubljana, "Central-Square", airQualityLocation: "E21"))
                .Add(Wroclaw, new CyanometerDataSource("Poland", "Wroclaw", AirQualitySource.Gios, Wroclaw, "University-Library", airQualityLocation: null));
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
        public string Country { get; }
        public string City { get; }
        public AirQualitySource AirQualitySource { get; }
        public string RootUriPath { get; }
        public string RootDiskPath { get; }
        public string CameraLocationPath { get; }
        public string AirQualityLocation { get; }
        public CyanometerDataSource(string country, string city, 
            AirQualitySource source, string rootUriPath, string cameraLocationPath, string airQualityLocation)
        {
            Country = country;
            City = city;
            AirQualitySource = source;
            RootUriPath = rootUriPath;
            RootDiskPath = Path.Combine(rootUriPath.Split('/'));
            CameraLocationPath = cameraLocationPath;
            AirQualityLocation = airQualityLocation;
        }
    }
}
