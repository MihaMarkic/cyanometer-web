using Cyanometer.Core;
using Cyanometer.Web.Models;
using System;
using System.Collections.Generic;

namespace Cyanometer.Web.Services.Abstract
{
    public interface IImagesManager
    {
        IEnumerable<ImageMeta> GetOlderImagesThan(CyanometerDataSource source, DateTimeOffset now);
    }
}
