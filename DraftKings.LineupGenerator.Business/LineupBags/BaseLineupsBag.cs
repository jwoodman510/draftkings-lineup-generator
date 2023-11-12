using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LinupBags
{
    public abstract class BaseLineupsBag : ConcurrentDictionary<decimal, ConcurrentDictionary<string, LineupModel>>
    {
        public string Description { get; }

        public BaseLineupsBag(string description)
        {
            Description = description;
        }

        public abstract LineupModel GetBestLineup();

        public abstract IEnumerable<LineupModel> GetBestLineups(int count);

        public abstract void UpdateLineups(ContestModel contest, LineupModel lineup, int max);

        public static string GetUniqueLineupId(LineupModel lineup)
        {
            return string.Join(".", lineup.Draftables.Select(x => x.Id).OrderBy(x => x));
        }
    }
}
