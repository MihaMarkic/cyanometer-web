using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Flurl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Cyanometer.Web.Services.Implementation
{
    public class ImagesManager : IImagesManager
    {
        static readonly JsonSerializerOptions options = new JsonSerializerOptions();
        readonly ILogger<ImagesManager> logger;
        readonly IWebHostEnvironment webHostEnvironment;
        readonly IImagesFileManager imagesFileManager;
        public ImagesManager(ILogger<ImagesManager> logger, IWebHostEnvironment webHostEnvironment, IImagesFileManager imagesFileManager)
        {
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.imagesFileManager = imagesFileManager;
        }

        public IEnumerable<ImageMeta> GetOlderImagesThan(CyanometerDataSource source, DateTimeOffset now)
        {
            string path = GetRelativePhysicalPathForDate(source, now);

            var fileProvider = webHostEnvironment.WebRootFileProvider;

            while (path != null)
            {
                string uriPath = GetRelativeUriPathForDate(source, now);
                var content = fileProvider.GetDirectoryContents(Path.Combine("cyano", path));
                var images = imagesFileManager.ReadMetaForDate(source, content, uriPath);
                foreach (var image in images.OrderByDescending(i => i.Date))
                {
                    yield return image;
                }
                path = GetRelativePhysicalPathForDate(source, now);
            }
        }

        internal string GetRelativePhysicalPathForDate(CyanometerDataSource source, DateTimeOffset now)
        {
            return Path.Combine(source.RootDiskPath, source.CameraLocationPath,
                now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"));
        }
        internal string GetRelativeUriPathForDate(CyanometerDataSource source, DateTimeOffset now)
        {
            return Url.Combine(source.RootUriPath, source.CameraLocationPath,
                now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"));
        }
    }
}
