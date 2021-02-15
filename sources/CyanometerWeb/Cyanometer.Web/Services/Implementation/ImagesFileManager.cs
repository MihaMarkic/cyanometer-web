using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Flurl;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Cyanometer.Web.Services.Implementation
{
    public class ImagesFileManager : IImagesFileManager
    {
        const string WwwRoot = "cyano";
        readonly IMemoryCache cache;
        readonly ILogger<ImagesFileManager> logger;
        public ImagesFileManager(ILogger<ImagesFileManager> logger, IMemoryCache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }
        public ImmutableArray<ImageMeta> ReadMetaForDate(CyanometerDataSource source, IDirectoryContents content, string uriPath, bool current)
        {
            string key = $"{CacheKeys.ImageFile}_{uriPath}";
            var result = cache.GetOrCreate(key, ce =>
            {
                logger.LogInformation("Missed cache on image files {key}", key);
                var query = from f in content
                            let ext = Path.GetExtension(f.Name)
                            where string.Equals(ext, ".info", System.StringComparison.Ordinal)
                            let fn = Path.GetFileNameWithoutExtension(f.Name)
                            let i = ReadImageInfoFromInfoFile(f)
                            orderby i.Date descending
                            select CreateMetaFromSourceAndInfo(source, i, uriPath, fn);
                ce.SetAbsoluteExpiration(current ? TimeSpan.FromMinutes(10) : TimeSpan.FromDays(1));
                return query.ToImmutableArray();
            });
            return result;
        }
        internal ImageMeta CreateMetaFromSourceAndInfo(CyanometerDataSource source, ImageInfo info, string uriPath, string fileName)
        {
            return new ImageMeta(
                            Url.Combine("/", WwwRoot, uriPath, $"{fileName}.jpg"),
                            Url.Combine("/", WwwRoot, uriPath, $"thumb-{fileName}.jpg"),
                            new DateTimeOffset(new DateTime(info.Date.Year, info.Date.Month, info.Date.Day, info.Date.Hour, info.Date.Minute, info.Date.Day)),
                            info.BluenessIndex
                        );
        }
        internal ImageInfo ReadImageInfoFromInfoFile(IFileInfo file)
        {
            return JsonConvert.DeserializeObject<ImageInfo>(ReadJsonFromFile(file));
        }

        internal string ReadJsonFromFile(IFileInfo file)
        {
            using (var sr = new StreamReader(file.CreateReadStream()))
            {
                return sr.ReadToEnd();
            }
        }

        public string GetRelativePhysicalPathForDate(CyanometerDataSource source, DateTimeOffset now)
        {
            return Path.Combine(source.RootDiskPath, source.CameraLocationPath,
                now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"));
        }

        internal static string CreateThumbnailFileName(string fileName) => $"thumb-{Path.GetFileName(fileName)}";
        internal static string CreateInfoFileName(string fileName) => $"{Path.GetFileNameWithoutExtension(fileName)}.info";
        public void SaveImage(CyanometerDataSource source, string wwwRootDirectory, Image<Rgba32> image, Image<Rgba32> thumb, ImageInfo info, string fullName)
        {
            string directory = Path.Combine(wwwRootDirectory, WwwRoot, GetRelativePhysicalPathForDate(source, info.Date));
            Directory.CreateDirectory(directory);
            image.Save(Path.Combine(directory, fullName));
            logger.LogDebug($"Image saved to {Path.Combine(directory, fullName)}");

            string thumbnailName = CreateThumbnailFileName(fullName);
            thumb.Save(Path.Combine(directory, thumbnailName));
            logger.LogDebug($"Image saved to {Path.Combine(directory, thumbnailName)}");

            string infoFileName = CreateInfoFileName(fullName);
            File.WriteAllText(Path.Combine(directory, infoFileName), JsonConvert.SerializeObject(info));
            logger.LogDebug($"Info saved to {Path.Combine(directory, infoFileName)}");
        }
    }
}
