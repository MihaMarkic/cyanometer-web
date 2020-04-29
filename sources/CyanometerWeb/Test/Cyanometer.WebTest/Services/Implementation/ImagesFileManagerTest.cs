using Cyanometer.Core;
using Cyanometer.Web.Services.Implementation;
using Microsoft.Extensions.FileProviders;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cyanometer.WebTest.Services.Implementation
{
    public class ImagesFileManagerTest: BaseTest<ImagesFileManager>
    {
        [TestFixture]
        public class ReadMetaForDate: ImagesFileManagerTest
        {
            [Test]
            public void Test()
            {
                var source = CyanometerDataSources.Default.Data[CyanometerDataSources.Ljubljana];
                string uriPath = "slovenia/ljubljana/somewhere/2017/08/02";
                var directoryContents = Substitute.For<IDirectoryContents>();
                string sample = "{\"Date\":\"2017-08-02T05:38:36+00:00\",\"BluenessIndex\":20}";
                var stream = WritesTextToMemoryStream(sample);
                var fileInfo = Substitute.For<IFileInfo>();
                fileInfo.CreateReadStream().Returns(stream);
                fileInfo.Name.Returns("sky-02.08.2017-05_23_36.info");
                var files = new List<IFileInfo> { fileInfo };
                directoryContents.GetEnumerator().Returns(files.GetEnumerator());

                var actual = Target.ReadMetaForDate(source, directoryContents, uriPath, current: true);

                Assert.That(actual.Length, Is.EqualTo(1));
                var single = actual[0];
                Assert.That(single.ImageUriPath, Is.EqualTo("/cyano/slovenia/ljubljana/somewhere/2017/08/02/sky-02.08.2017-05_23_36.jpg"));
                Assert.That(single.ThumbnailUriPath, Is.EqualTo("/cyano/slovenia/ljubljana/somewhere/2017/08/02/thumb-sky-02.08.2017-05_23_36.jpg"));
            }
        }

        [TestFixture]
        public class ReadImageInfoFromInfoFile: ImagesFileManagerTest
        {
            [Test]
            public void WhenGivenFile_DeserializesCorrectly()
            {
                string sample = "{\"Date\":\"2017-08-02T05:38:36+00:00\",\"BluenessIndex\":20}";
                var stream = WritesTextToMemoryStream(sample);
                var fileInfo = Substitute.For<IFileInfo>();
                fileInfo.CreateReadStream().Returns(stream);

                var actual = Target.ReadImageInfoFromInfoFile(fileInfo);

                Assert.That(actual.Date, Is.EqualTo(new DateTimeOffset(2017, 8, 2, 5, 38, 36, TimeSpan.Zero)));
                Assert.That(actual.BluenessIndex, Is.EqualTo(20));
            }
        }

        [TestFixture]
        public class CreateMetaFromSourceAndInfo : ImagesFileManagerTest
        {
            [Test]
            public void WithGivenData_CreatesImageMetaCorrectly()
            {
                var source = CyanometerDataSources.Default.Data[CyanometerDataSources.Ljubljana];
                var actual = Target.CreateMetaFromSourceAndInfo(
                    source,
                    new Web.Models.ImageInfo(new DateTimeOffset(2017, 8, 2, 5, 38, 36, TimeSpan.Zero), 20),
                    "slovenia/ljubljana/somewhere/2017/08/02",
                    "sky-02.08.2017-05_23_36"
                );

                Assert.That(actual.ImageUriPath, Is.EqualTo("/cyano/slovenia/ljubljana/somewhere/2017/08/02/sky-02.08.2017-05_23_36.jpg"));
                Assert.That(actual.ThumbnailUriPath, Is.EqualTo("/cyano/slovenia/ljubljana/somewhere/2017/08/02/thumb-sky-02.08.2017-05_23_36.jpg"));
                Assert.That(actual.Date, Is.EqualTo(new DateTimeOffset(2017, 8, 2, 5, 38, 36, TimeSpan.Zero)));
                Assert.That(actual.BluenessIndex, Is.EqualTo(20));
            }
        }

        [TestFixture]
        public class GetRelativePhysicalPathForDate : ImagesFileManagerTest
        {
            [Test]
            public void GivenSourceAndDate_ConstructsPathCorrectly()
            {
                var ljubljana = new CyanometerDataSource(Guid.NewGuid(), "Slovenia", "Ljubljana", AirQualitySource.Arso,
                    rootUriPath: "slovenia/ljubljana", cameraLocationPath: "Central-Square", "E21");

                var actual = Target.GetRelativePhysicalPathForDate(ljubljana, new DateTimeOffset(2017, 8, 2, 5, 38, 36, TimeSpan.Zero));

                Assert.That(actual, Is.EqualTo(@"slovenia\ljubljana\Central-Square\2017\08\02"));
            }
        }
    }
}
