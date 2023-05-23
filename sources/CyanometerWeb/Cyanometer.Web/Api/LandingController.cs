using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Cyanometer.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandingController : CyanometerController<LandingController>
    {
        readonly IImagesManager imagesManager;

        public LandingController(ILogger<LandingController> logger, IImagesManager imagesManager): base(logger)
        {
            this.imagesManager = imagesManager;
        }

        [HttpGet]
        public ActionResult<ImageItem[]> Get()
        {
            DateTimeOffset now;
//#if DEBUG
//            now = new DateTimeOffset(2017, 08, 03, 12, 12, 12, TimeSpan.Zero);
//#else
            now = DateTimeOffset.Now;
//#endif
            var query = from s in CyanometerDataSources.Default.Data.Values
                        let m = imagesManager.GetOlderImagesThan(s, now).Cast<ImageMeta?>().FirstOrDefault()
                        where m.HasValue
                        select new ImageItem(
                                 takenAt: m.Value.Date,
                                 url: m.Value.ImageUriPath,
                                 thumbnailUrl: m.Value.ThumbnailUriPath,
                                 city: s.City,
                                 cityUrl: s.City.Split(' ')[0],
                                 country: s.Country,
                                 id: 146649,
                                 bluenessIndex: m.Value.BluenessIndex
                             );

            var result = query.ToArray();
            return result;
        }
    }
}
