using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class CommonFilters
    {
        public static IEnumerable<DraftableModel> ExcludeDisabled(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => !x.IsDisabled);
    }
}
