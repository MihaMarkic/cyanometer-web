using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Cyanometer.Web.Utilities;
using Flurl;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        readonly IMemoryCache cache;
        readonly Calculator calculator;
        public ImagesManager(ILogger<ImagesManager> logger, IWebHostEnvironment webHostEnvironment, IImagesFileManager imagesFileManager,
            IMemoryCache cache)
        {
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.imagesFileManager = imagesFileManager;
            this.cache = cache;
            calculator = new Calculator(new NearestColorCalculator());
        }

        public IEnumerable<ImageMeta> GetOlderImagesThan(CyanometerDataSource source, DateTimeOffset now)
        {
            string path = imagesFileManager.GetRelativePhysicalPathForDate(source, now);

            var fileProvider = webHostEnvironment.WebRootFileProvider;

            bool isToday = true;
            logger.LogDebug($"Checking images for {fileProvider.GetFileInfo(Path.Combine("cyano", path)).PhysicalPath}");
            var end = now.AddYears(-1);
            while (now > end)
            {
                var content = fileProvider.GetDirectoryContents(Path.Combine("cyano", path));
                string uriPath = GetRelativeUriPathForDate(source, now);
                var images = imagesFileManager.ReadMetaForDate(source, content, uriPath, isToday);
                foreach (var image in images.OrderByDescending(i => i.Date))
                {
                    yield return image;
                }
                now = now.AddDays(-1);
                isToday = false;
                path = imagesFileManager.GetRelativePhysicalPathForDate(source, now);
            }
        }

        internal string GetRelativeUriPathForDate(CyanometerDataSource source, DateTimeOffset now)
        {
            return Url.Combine(source.RootUriPath, source.CameraLocationPath,
                now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"));
        }

        public void SaveImage(CyanometerDataSource source, string fileName, Stream stream)
        {
            string safeFileName = Path.GetFileName(fileName);
            DateTimeOffset takenAt = DateFromFileName(safeFileName);
            logger.LogDebug($"File date is {takenAt}");
            using (var image = Image.Load<Rgba32>(stream))
            using (var thumb = image.Clone())
            {
                // also crops to avoid including random objects on the edge of the photo
                // factor determines the crop ratio. Factor 2 means it crops out 50% of image
                float factor = 2;
                const int thumbWidth = 800;
                const int thumbHeight = 600;
                thumb.Mutate(x => x
                    .Resize((int)(thumbWidth * factor), (int)(thumbHeight * factor))
                    .Crop(new Rectangle((int)(thumbWidth / factor / 2), (int)(thumbWidth / factor / 2), thumbWidth, thumbHeight)));
                var colors = CollectColors(thumb);
                var info = new ImageInfo(
                                takenAt,
                                calculator.GetBluenessIndexTopPixels(colors, 30).Index
                            );
                logger.LogDebug("Thumbnail created, saving");
                var fileProvider =(PhysicalFileProvider)webHostEnvironment.WebRootFileProvider;
                imagesFileManager.SaveImage(source, fileProvider.Root, image, thumb, info, safeFileName);
            }
        }

        internal static DateTimeOffset DateFromFileName(string fileName)
        {
            return new DateTimeOffset(
                int.Parse(fileName.Substring(10, 4)),
                int.Parse(fileName.Substring(7, 2)),
                int.Parse(fileName.Substring(4, 2)),
                int.Parse(fileName.Substring(15, 2)),
                int.Parse(fileName.Substring(18, 2)),
                int.Parse(fileName.Substring(21, 2)),
                TimeSpan.Zero
            );
        }

        ImmutableArray<Utilities.Color> CollectColors(Image<Rgba32> image)
        {
            try
            {
                var colors = new List<Utilities.Color>(image.Width * image.Height);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixel = image[x, y];
                        colors.Add(Utilities.Color.FromRgb(pixel.R, pixel.G, pixel.B));
                    }
                }
                return colors.ToImmutableArray();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed extracting colors from image");
                throw;
            }
        }
    }
}
