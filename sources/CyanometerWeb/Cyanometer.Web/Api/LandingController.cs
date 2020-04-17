using Flurl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;

namespace Cyanometer.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandingController : ControllerBase
    {
        readonly ILogger<LandingController> logger;

        public LandingController(ILogger<LandingController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<LandingItem[]> Get()
        {
            return new[]
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
                    takenAt: DateTimeOffset.Now,
                    url: "https://s3.eu-central-1.amazonaws.com/cyanometer-v2/prod/Slovenia/Ljubljana/Central-Square/2020/04/10/sky-10.04.2020-12_19_53-large.jpg",
                    city: "Ljubljana",
                    country: "Slovenija",
                    id: 146648,
                    bluenessIndex: 19
                )
           };
        }
    }

    public readonly struct LandingItem
    {
        public DateTimeOffset TakenAt { get; }
        public string Url { get; }
        public string ThumbnailUrl { get; }
        public string City { get; }
        public string Country { get; }
        public int Id { get; }
        public int BluenessIndex { get; }
        public LandingItem(DateTimeOffset takenAt, string url, string city, string country, int id, int bluenessIndex)
        {
            TakenAt = takenAt;
            Url = ImageLocator.GetFullUrl(url); ;
            City = city;
            Country = country;
            Id = id;
            BluenessIndex = bluenessIndex;
            ThumbnailUrl = ImageLocator.GetThumbnailUrl(url);
        }
    }

    public static class ImageLocator
    {
        const string rootUrl = "https://res.cloudinary.com/mota/image/fetch/";
        static readonly string thumbnailRootUrl = Url.Combine(rootUrl, "w_178,h_100,c_thumb");
        static readonly string fullRootUrl = Url.Combine(rootUrl, "w_1200,h_675,ar_16:9");
        public static string GetThumbnailUrl(string imageUrl) => Url.Combine(thumbnailRootUrl, imageUrl);
        public static string GetFullUrl(string imageUrl) => Url.Combine(fullRootUrl, imageUrl);
    }
}
