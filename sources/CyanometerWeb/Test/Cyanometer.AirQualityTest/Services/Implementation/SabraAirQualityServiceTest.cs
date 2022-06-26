using Cyanometer.AirQuality;
using Cyanometer.AirQuality.Services.Implementation.Specific;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Xml.Linq;

namespace Cyanometer.AirQualityTest.Services.Implementation;

public partial class SabraAirQualityServiceTest: BaseTest<SabraAirQualityService>
{
    static readonly string Root = Path.Combine("Samples", "Sabra");
    internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine(Root, file));
    [TestFixture]
    public class GetValueFor: SabraAirQualityServiceTest
    {
        XElement sample = default!;
        ILogger logger = default!;

        public override void SetUp()
        {
            logger = Substitute.For<ILogger>();
            sample = new XElement("messstelle",
                new XElement("kanalgruppe", new XAttribute("kanalgruppetyp", "NO2"),
                    new XElement("kanal",
                        new XElement("messwert",
                            new XAttribute("wert", "14.4413560343318"),
                            new XAttribute("endzeit", "23.06.2022 13:00:00")),
                    new XElement("messwert",
                            new XAttribute("wert", "15.6846515869564"),
                            new XAttribute("endzeit", "23.06.2022 14:00:00")),
                    new XElement("messwert"))),
                new XElement("kanalgruppe", new XAttribute("kanalgruppetyp", "O3"),
                    new XElement("kanal",
                        new XElement("messwert",
                            new XAttribute("wert", "82.9127026102197"),
                            new XAttribute("endzeit", "23.06.2022 13:00:00")),
                    new XElement("messwert",
                            new XAttribute("wert", "84.7086456270476"),
                            new XAttribute("endzeit", "23.06.2022 14:00:00")),
                    new XElement("messwert"))));
            base.SetUp();
        }
        [Test]
        public void GivenSampleElement_SelectNO2_ReturnsItsValue()
        {
            var actual = SabraAirQualityService.GetValueFor(logger, sample, "NO2");

            Assert.That(actual, Is.EqualTo(15.6846515869564));
        }
        [Test]
        public void GivenSampleElement_SelectO3_ReturnsItsValue()
        {
            var actual = SabraAirQualityService.GetValueFor(logger, sample, "O3");

            Assert.That(actual, Is.EqualTo(84.7086456270476));
        }
    }
    [TestFixture]
    public class GetDate : SabraAirQualityServiceTest
    {
        ILogger logger = default!;
        public override void SetUp()
        {
            logger = Substitute.For<ILogger>();
            base.SetUp();
        }
        [Test]
        public void GivenSample_ExtractsDate()
        {
            var source = new XElement("selektion", new XAttribute("ende", "24.06.2022 11:00:00"));

            var actual = SabraAirQualityService.GetDate(logger, source);

            Assert.That(actual, Is.EqualTo(new DateTime(2022, 6, 24, 11, 0, 0)));
        }
    }
    [TestFixture]
    public class ParseData: SabraAirQualityServiceTest
    {
        ILogger logger = default!;
        public override void SetUp()
        {
            logger = Substitute.For<ILogger>();
            base.SetUp();
        }
        [Test]
        public void GivenSample_ReturnsCorrectData()
        {
            var doc = XDocument.Parse(GetSampleContent("sample.xml"));

            var actual = SabraAirQualityService.ParseData(logger, doc, null);

            var expected = new AirQualityData
            {
                Date = new DateTime(2022, 6, 24, 11, 0, 0),
                NO2 = 42.9415156252649,
                O3 = 22.0501892621643,
                PM10 = 12.75,
                SO2 = null,
            };
            Assert.That(actual, Is.EqualTo(expected).Using(AirQualityDataEqualityComparer.Default));
        }
    }
}
