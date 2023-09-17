using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LinupBags
{
    public abstract class BaseLineupsBag : ConcurrentDictionary<decimal, ConcurrentDictionary<string, LineupModel>>
    {
        public abstract LineupModel GetBestLineup();

        public abstract IEnumerable<LineupModel> GetBestLineups(int count);

        public static string GetUniqueLineupId(LineupModel lineup)
        {
            return string.Join(".", lineup.Draftables.Select(x => x.Id).OrderBy(x => x));
        }
    }
}
