using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupBags
{
    public class ProjectedPointsLineupsBag : BaseLineupsBag
    {
        public ProjectedPointsLineupsBag() : base("Projected FPPG") { }

        public override LineupModel GetBestLineup()
        {
            if (Keys.Count == 0)
            {
                return null;
            }

            var maxKey = Keys.Max();
            return this[maxKey].OrderByDescending(x => x.Value.ProjectedFppg).FirstOrDefault().Value;
        }

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

        public override void UpdateLineups(LineupModel lineup, int max)
        {
            var minKey = Keys.Count == 0 ? 0 : Keys.Min();

            if (lineup.ProjectedFppg < minKey)
            {
                return;
            }

            var lineups = GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentDictionary<string, LineupModel>());

            lineups.TryAdd(GetUniqueLineupId(lineup), lineup);

            if (Keys.Count > max)
            {
                TryRemove(minKey, out _);
            }
        }
    }
}
