using Cyanometer.Core;
using Cyanometer.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cyanometer.Web.Services.Abstract
{
    public interface IImagesManager
    {
        IEnumerable<ImageMeta> GetOlderImagesThan(CyanometerDataSource source, DateTimeOffset now);
        void SaveImage(CyanometerDataSource source, string fileName, Stream stream);
    }
}
