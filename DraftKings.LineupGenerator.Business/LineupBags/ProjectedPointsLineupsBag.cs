using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupBags
{
    public class ProjectedPointsLineupsBag : BaseLineupsBag
    {
        public ProjectedPointsLineupsBag(string description) : base(description) { }

        public override IEnumerable<LineupModel> GetBestLineups(int count)
        {
            var bestLineups = new List<LineupModel>();

            foreach (var lineup in Values.SelectMany(x => x.Values).OrderByDescending(x => x.ProjectedFppg))
            {
                if (bestLineups.Count == count)
                {
                    break;
                }

                if (!bestLineups.Any(x => GetUniqueLineupId(x) == GetUniqueLineupId(lineup)))
                {
                    bestLineups.Add(lineup);
                }
            }

            return bestLineups;
        }

        public override void UpdateLineups(ContestModel contest, LineupModel lineup, int max)
        {
            var keys = Keys;
            var minKey = keys.Min();

            if (lineup.ProjectedFppg < minKey)
            {
                return;
            }

            var lineups = GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentDictionary<string, LineupModel>());

            lineups.TryAdd(GetUniqueLineupId(lineup), lineup);

            if (keys.Count + 1 > max)
            {
                TryRemove(minKey, out _);
            }
        }
    }
}
