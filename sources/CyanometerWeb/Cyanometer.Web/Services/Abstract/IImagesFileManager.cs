using Cyanometer.Core;
using Cyanometer.Web.Models;
using Microsoft.Extensions.FileProviders;
using System.Collections.Immutable;

namespace Cyanometer.Web.Services.Abstract
{
    public interface IImagesFileManager
    {
        ImmutableArray<ImageMeta> ReadMetaForDate(CyanometerDataSource source, IDirectoryContents content, string uriPath, bool current);
    }
}
