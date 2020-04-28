using Cyanometer.AirQuality.Services.Implementation.Specific;
using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Cyanometer.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : CyanometerController<ImagesController>
    {
        readonly IImagesManager imagesManager;
        public ImagesController(ILogger<ImagesController> logger, 
            IImagesManager imagesManager) : base(logger)
        {
            this.imagesManager = imagesManager;
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok();
        }

        [HttpGet]
        [Route("{country}/{city}")]
        public ActionResult<DisplayData> Get(string country, string city)
        {
            var dataSource = CyanometerDataSources.Default.GetData(city, country);
            if (dataSource != null)
            {
                DateTimeOffset now;
#if DEBUG
                now = new DateTimeOffset(2017, 08, 03, 12, 12, 12, TimeSpan.Zero);
#else
                now = DateTimeOffset.Now;
#endif
                var lastImages = imagesManager.GetOlderImagesThan(dataSource, now).Take(12);
                var query = from m in lastImages
                            select new ImageItem(
                                     takenAt: m.Date,
                                     url: m.ImageUriPath,
                                     thumbnailUrl: m.ThumbnailUriPath,
                                     city: city,
                                     country: country,
                                     id: 146649,
                                     bluenessIndex: m.BluenessIndex
                                 );
                var imageItems = query.ToImmutableArray();
                var averageBlueness = Convert.ToInt32(imageItems.Average(i => i.BluenessIndex));
                return new DisplayData(
                    averageBlueness: averageBlueness,
                    images: imageItems
                );
            }
            return NotFound();
        }
    }
}