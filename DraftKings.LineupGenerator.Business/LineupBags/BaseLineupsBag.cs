using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Business.LinupBags
{
    public abstract class BaseLineupsBag : ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>>
    {
        public abstract LineupModel GetBestLineup();
    }
}
