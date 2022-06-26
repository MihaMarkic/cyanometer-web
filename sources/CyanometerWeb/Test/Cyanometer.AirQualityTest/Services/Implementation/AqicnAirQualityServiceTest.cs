using Cyanometer.AirQuality;
using Cyanometer.AirQuality.Services.Implementation.Specific;
using NUnit.Framework;
using static Cyanometer.AirQualityTest.Services.Implementation.SabraAirQualityServiceTest;

namespace Cyanometer.AirQualityTest.Services.Implementation;

public class AqicnAirQualityServiceTest: BaseTest<AqicnAirQualityService>
{
    static readonly string Root = Path.Combine("Samples", "Aqicn");
    internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine(Root, file));
    [TestFixture]
    public class DeserializeData : AqicnAirQualityServiceTest
    {
        [Test]
        public void GivenSample_DeserializesCorrectly()
        {
            string content = GetSampleContent("sample.json");

            var actual = AqicnAirQualityService.DeserializeData(content);

            var expected = new AirQualityData
            {
                Date = new DateTime(2022, 6, 26, 10, 0, 0),
                NO2 = 2.3,
                O3 = 44.3,
                PM10 = 11,
                SO2 = 1.4,
            };
            Assert.That(actual.Data.Time.Iso, 
                Is.EqualTo(new DateTimeOffset(2022, 6, 26, 10, 0, 0, TimeSpan.FromHours(2))));
            Assert.That(actual.Data.Iaqi.No2.V, Is.EqualTo(2.3));
            Assert.That(actual.Data.Iaqi.O3.V, Is.EqualTo(44.3));
            Assert.That(actual.Data.Iaqi.Pm10.V, Is.EqualTo(11));
            Assert.That(actual.Data.Iaqi.So2.V, Is.EqualTo(1.4));
        }
    }
}
