using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.LinupBags
{
    public abstract class BaseLineupsBag : ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>>
    {
        public abstract LineupModel GetBestLineup();

        public abstract IEnumerable<LineupModel> GetBestLineups(int count);
    }
}
