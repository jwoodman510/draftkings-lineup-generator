using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class DraftStatFilter
    {
        public static IEnumerable<DraftableModel> ExcludeZeroFppg(this IEnumerable<DraftableModel> draftables, List<DraftStatModel> draftStats)
        {
            return draftables.Where(x => x.GetFppg(draftStats) > 0);
        }

        public static IEnumerable<DraftableModel> MinimumFppg(this IEnumerable<DraftableModel> draftables, List<DraftStatModel> draftStats, decimal fppg)
        {
            return draftables.Where(x => x.GetFppg(draftStats) >= fppg);
        }
    }
}
