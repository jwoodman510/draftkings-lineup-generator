using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class SalaryFilter
    {
        public static IEnumerable<DraftableModel> ExcludeZeroSalary(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Salary > 0);

        public static IEnumerable<DraftableModel> ExcludeBaseSalary(this IEnumerable<DraftableModel> draftables)
        {
            var baseSalaries = draftables.GroupBy(x => x.RosterSlotId).ToDictionary(x => x.Key, x => x.Min(y => y.Salary));

            return draftables.Where(x => x.Salary > baseSalaries[x.RosterSlotId]);
        }
    }
}
