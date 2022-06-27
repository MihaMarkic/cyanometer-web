using System.Xml.Linq;
using Cyanometer.AirQuality.Services.Implementation.Specific;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Cyanometer.AirQualityTest.Services.Implementation
{
    public class SachsenAirQualityServiceTest: BaseTest<SachsenAirQualityService>
    {
        static readonly string Root = Path.Combine("Samples", "Sachsen");
        internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine(Root, file));
        [TestFixture]
        public class GetDate: SachsenAirQualityServiceTest
        {
            [Test]
            public void GivenSample_ReturnsDate()
            {
                var actual = SachsenAirQualityService.GetDate("2022-06-27T00:00:00", "15:00");

                Assert.That(actual, Is.EqualTo(new DateTime(2022, 6, 27, 15, 00, 00)));
            }
        }
        [TestFixture]
        public class ParseData: SachsenAirQualityServiceTest
        {
            ILogger logger = default!;
            public override void SetUp()
            {
                logger = Substitute.For<ILogger>();
                base.SetUp();
            }
            [Test]
            public void GivenSampleResponse_ReturnsCorrectData()
            {
                var doc = XDocument.Parse(GetSampleContent("sample.xml"));
                var actual = SachsenAirQualityService.ParseData(logger, doc, "");

                Assert.That(actual.Date, Is.EqualTo(new DateTime(2022, 6, 27, 14, 00, 00)));
                Assert.That(actual.O3, Is.EqualTo(116.341));
                Assert.That(actual.PM10, Is.EqualTo(12.259));
                Assert.That(actual.SO2, Is.EqualTo(1.493));
                Assert.That(actual.NO2, Is.EqualTo(4.449));
            }
        }
    }
}
