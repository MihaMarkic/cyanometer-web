using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;

namespace Cyanometer.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : CyanometerController<ImagesController>
    {
        //readonly IFileProvider fileProvider;
        public ImagesController(ILogger<ImagesController> logger) : base(logger)
        {
            //this.fileProvider = fileProvider;
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok();
        }

        //[HttpGet]
        //[Route("last-image-large-thumbnail/{source}")]
        //public IActionResult GetLatestImageLargeThumbnail(string source)
        //{
        //    var fileInfo = fileProvider.GetFileInfo($"/images/test_{source}.jpg");
        //    return PhysicalFile($"~/wwwroot/images/test_{source}.jpg", "image/jpeg");
        //}

        [HttpGet]
        [Route("{country}/{city}")]
        public ActionResult<DisplayData> Get(string country, string city)
        {
            return new DisplayData(
                archiveUrl: "https://blog.rthand.com/",
                averageBlueness: 29,
                images: new[]
                {
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(15),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 34
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(30),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(45),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(60),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(75),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(90),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(105),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(120),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now.AddMinutes(135),
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: 19
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                )
           }.ToImmutableArray());
        }
    }

    public readonly struct DisplayData
    {
        public string ArchiveUrl { get; }
        public int AverageBlueness { get; }
        public ImmutableArray<LandingItem> Images { get; }
        public DisplayData(string archiveUrl, int averageBlueness, ImmutableArray<LandingItem> images)
        {
            Images = images;
            ArchiveUrl = archiveUrl;
            AverageBlueness = averageBlueness;
        }
    }
}