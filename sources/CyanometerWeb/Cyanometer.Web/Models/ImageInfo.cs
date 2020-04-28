using System;

namespace Cyanometer.Web.Models
{
    public readonly struct ImageInfo
    {
        public DateTimeOffset Date { get; }
        public int BluenessIndex { get; }
        public ImageInfo(DateTimeOffset date, int bluenessIndex)
        {
            Date = date;
            BluenessIndex = bluenessIndex;
        }
    }
}
