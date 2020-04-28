using System;

namespace Cyanometer.Web.Models
{
    public readonly struct ImageItem
    {
        public DateTimeOffset TakenAt { get; }
        public string Url { get; }
        public string ThumbnailUrl { get; }
        public string City { get; }
        public string Country { get; }
        public int Id { get; }
        public int BluenessIndex { get; }
        public ImageItem(DateTimeOffset takenAt, string url, string thumbnailUrl, string city, string country, int id, int bluenessIndex)
        {
            TakenAt = takenAt;
            Url = url;
            City = city;
            Country = country;
            Id = id;
            BluenessIndex = bluenessIndex;
            ThumbnailUrl = thumbnailUrl;
        }
    }
}
