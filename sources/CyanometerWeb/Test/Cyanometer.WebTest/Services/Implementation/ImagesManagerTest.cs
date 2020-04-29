using Cyanometer.Web.Services.Implementation;
using NUnit.Framework;
using System;

namespace Cyanometer.WebTest.Services.Implementation
{
    public class ImagesManagerTest: BaseTest<ImagesManager>
    {
        [TestFixture]
        public class DateFromFileName: ImagesManagerTest
        {
            [Test]
            public void GivenFileName_ExtractsDateCorrectly()
            {
                var actual = ImagesManager.DateFromFileName("sky-21.04.2020-06_19_58.jpg");

                Assert.That(actual, Is.EqualTo(new DateTimeOffset(2020, 4, 21, 6, 19, 58, TimeSpan.Zero)));
            }
        }
    }
}
