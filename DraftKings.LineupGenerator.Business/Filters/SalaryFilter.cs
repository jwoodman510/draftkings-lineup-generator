﻿using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class SalaryFilter
    {
        public static IEnumerable<DraftableModel> ExcludeZeroSalary(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => x.Salary > 0);
    }
}
