using System.Collections.Immutable;

namespace Cyanometer.Web.Models
{
    public readonly struct DisplayData
    {
        public int AverageBlueness { get; }
        public ImmutableArray<ImageItem> Images { get; }
        public DisplayData(int averageBlueness, ImmutableArray<ImageItem> images)
        {
            Images = images;
            AverageBlueness = averageBlueness;
        }
    }
}