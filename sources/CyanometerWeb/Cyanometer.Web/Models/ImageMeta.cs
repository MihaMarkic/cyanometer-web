using System;

namespace Cyanometer.Web.Models
{
    public readonly struct ImageMeta
    {
        public string ImageUriPath { get; }
        public string ThumbnailUriPath { get; }
        public DateTimeOffset Date { get; }
        public int BluenessIndex { get; }
        public ImageMeta(string imageUriPath, string thumbnailUriPath, DateTimeOffset date, int bluenessIndex)
        {
            ImageUriPath = imageUriPath;
            ThumbnailUriPath = thumbnailUriPath;
            Date = date;
            BluenessIndex = bluenessIndex;
        }
    }
}
