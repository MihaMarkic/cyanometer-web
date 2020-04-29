using Cyanometer.Core;
using Cyanometer.Web.Models;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Immutable;

namespace Cyanometer.Web.Services.Abstract
{
    public interface IImagesFileManager
    {
        ImmutableArray<ImageMeta> ReadMetaForDate(CyanometerDataSource source, IDirectoryContents content, string uriPath, bool current);
        void SaveImage(CyanometerDataSource source, string wwwRoot, Image<Rgba32> image, Image<Rgba32> thumb, ImageInfo info, string fullName);
        string GetRelativePhysicalPathForDate(CyanometerDataSource source, DateTimeOffset now);
    }
}
