using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupBags
{
    public class ProjectedPointsLineupsBag : BaseLineupsBag
    {
        public override LineupModel GetBestLineup()
        {
            var maxKey = Keys.Max();
            return this[maxKey].OrderByDescending(x => x.ProjectedFppg).FirstOrDefault();
        }

        public void UpdateLineups(LineupModel lineup)
        {
            var minKey = Keys.Count == 0 ? 0 : Keys.Min();

            if (lineup.ProjectedFppg < minKey)
            {
                return;
            }

            var lineups = GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentBag<LineupModel>());

            if (lineups.Count < 5)
            {
                lineups.Add(lineup);
            }

            if (Keys.Count > 5)
            {
                TryRemove(minKey, out _);
            }
        }
    }
}
