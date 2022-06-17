using System;
using System.Collections.Immutable;
using System.IO;

namespace Cyanometer.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Location Tokens are not under SC and are stored in CyanometerDataSources.nogit.cs file (or other not under SC).
    /// </remarks>
    public partial class CyanometerDataSources
    {
        public const string Wroclaw = "poland/wroclaw";
        public const string Ljubljana = "slovenia/ljubljana";
        public const string Dresden = "germany/dresden";
        public const string Geneva = "switzerland/geneva";
        public static CyanometerDataSources Default { get; } = new CyanometerDataSources();
        public ImmutableDictionary<string, CyanometerDataSource> Data { get; }
        CyanometerDataSources()
        {
            Data = ImmutableDictionary.Create<string, CyanometerDataSource>(StringComparer.OrdinalIgnoreCase)
                .Add(Ljubljana, new CyanometerDataSource(Guid.Parse(LjubljanaToken), "Slovenia", "Ljubljana", 
                    AirQualitySource.Arso, Ljubljana, "Central-Square", airQualityLocation: "E403"))
                .Add(Wroclaw, new CyanometerDataSource(Guid.Parse(WroclawToken), "Poland", "Wroclaw", 
                    AirQualitySource.Gios, Wroclaw, "University-Library", airQualityLocation: null))
                .Add(Dresden, new CyanometerDataSource(Guid.Parse(DresdenToken), "Germany", "Dresden",
                    AirQualitySource.Sachsen, Dresden, "University-Library", airQualityLocation: "DESN083"))
                .Add(Geneva, new CyanometerDataSource(Guid.Parse(GenevaToken), "Switzerland", "Geneva",
                    AirQualitySource.Geneva, Geneva, "University-Library", airQualityLocation: null));
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
        public Guid Id { get; }
        public string Country { get; }
        public string City { get; }
        public AirQualitySource AirQualitySource { get; }
        public string RootUriPath { get; }
        public string RootDiskPath { get; }
        public string CameraLocationPath { get; }
        public string AirQualityLocation { get; }
        public CyanometerDataSource(Guid id, string country, string city, 
            AirQualitySource source, string rootUriPath, string cameraLocationPath, string airQualityLocation)
        {
            Id = id;
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
