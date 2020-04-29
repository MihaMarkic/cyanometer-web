using System.Collections.Immutable;
using System.Diagnostics;

namespace Cyanometer.Web.Utilities
{
    [DebuggerDisplay("{Index}")]
    public readonly struct GetBluenessIndexResult
    {
        public int Index { get; }
        public ImmutableArray<int> Indexes { get; }
        public GetBluenessIndexResult(int index, ImmutableArray<int> indexes)
        {
            Index = index;
            Indexes = indexes;
        }
    }
}
