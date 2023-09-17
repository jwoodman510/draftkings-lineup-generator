using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public override IEnumerable<LineupModel> GetBestLineups(int count)
        {
            var bestLineups = new List<LineupModel>();

            foreach (var lineup in Values.SelectMany(x => x).OrderByDescending(x => x.ProjectedFppg))
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

        public void UpdateLineups(LineupModel lineup, int max)
        {
            var minKey = Keys.Count == 0 ? 0 : Keys.Min();

            if (lineup.ProjectedFppg < minKey)
            {
                return;
            }

            var lineups = GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentBag<LineupModel>());

            if (lineups.Count < max && !lineups.Any(x => GetUniqueLineupId(x) == GetUniqueLineupId(lineup)))
            {
                lineups.Add(lineup);
            }

            if (Keys.Count > max)
            {
                TryRemove(minKey, out _);
            }
        }

        private static string GetUniqueLineupId(LineupModel lineup)
        {
            return string.Join(".", lineup.Draftables.Select(x => x.Id).OrderBy(x => x));
        }
    }
}
