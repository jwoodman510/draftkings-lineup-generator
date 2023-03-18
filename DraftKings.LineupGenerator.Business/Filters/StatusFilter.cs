using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class StatusFilter
    {
        public static IEnumerable<DraftableModel> ExcludeDoubtful(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Status != Statuses.Doubtful);

        public static IEnumerable<DraftableModel> ExcludeInjuredReserve(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Status != Statuses.InjuredReserve);

        public static IEnumerable<DraftableModel> ExcludeOut(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Status != Statuses.Out);

        public static IEnumerable<DraftableModel> ExcludeQuestionable(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Status != Statuses.Questionable);
    }
}
