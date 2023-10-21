using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupBags
{
    public class ProjectedPointsLineupsBag : BaseLineupsBag
    {
        private decimal MinKey => Keys.Count == 0 ? 0 : Keys.Min();

        public ProjectedPointsLineupsBag(string description) : base(description) { }

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
            if (lineup.ProjectedFppg < MinKey)
            {
                return;
            }

            var lineups = GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentDictionary<string, LineupModel>());

            lineups.TryAdd(GetUniqueLineupId(lineup), lineup);

            if (Keys.Count > max)
            {
                TryRemove(MinKey, out _);
            }
        }
    }
}
