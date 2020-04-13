using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;

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
        public ActionResult<LandingItem[]> Get(string country, string city)
        {
            return new[]
            {
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Poland/Wroclaw/University Library/2020/04/10/sky-10.04.2020-10_21_49-large.jpg",
                    city: "Wroclaw",
                    country: "Poland",
                    id: 146649,
                    bluenessIndex: "19"
                ),
                new LandingItem(
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: "19"
                )
           };
        }
    }
}