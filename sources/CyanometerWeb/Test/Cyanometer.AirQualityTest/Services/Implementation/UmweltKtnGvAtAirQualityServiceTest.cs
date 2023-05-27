using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Cyanometer.AirQualityTest.Services.Implementation;
public class UmweltKtnGvAtAirQualityServiceTest: BaseTest<UmweltKtnGvAtAirQualityService>
{
    [TestFixture]
    public class ParseData: UmweltKtnGvAtAirQualityServiceTest
    {
        [Test]
        public void GivenSampleSimpleData_MeasurementValueIsCorrect()
        {
            string source = """
                >02SP18PM10      01HMW     �g/m3     Spittal - 10. Oktober Strasse
                202305191700130.06
                ==ENDE==
                """;

            var actual = UmweltKtnGvAtAirQualityService.ParseData(Substitute.For<ILogger>(), source.Split(Environment.NewLine), "SP18");

            var actualStation = actual[0];
            var actualMeasurement = actualStation.Measurements[0];
            Assert.That(actualMeasurement.Value, Is.EqualTo(0.06));
            //Assert.That(actual[0], Is.EqualTo(
            //    new Station
            //    {
            //        MeasurementNetworkIdentifier = "02",
            //        StationIdentifier = "M121",
            //        ComponentCode = "SO2",
            //        ComponentSubNumber = "01",
            //        TimeSeriesType = "HMW",
            //        MeasuredValueUnit = "�g/m3",
            //        Description = "Arnoldstein - Gailitz",
            //        Measurements = new Measurement[]
            //        {
            //            new Measurement
            //            {
            //                Date = new DateTime(2023, 05, 19, 17, 00, 0),
            //                ControlLevel = ControlLevel.Unverified,
            //                Validity = Validity.Valid,
            //                Value = 30.06
            //            }
            //        }
            //    }
            //));
        }
    }
}
