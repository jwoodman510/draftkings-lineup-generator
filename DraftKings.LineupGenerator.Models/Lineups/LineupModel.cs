using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupModel
    {
        public decimal Salary { get; }

        public decimal Fppg { get; }

        public decimal ProjectedFppg { get; }

        public List<DraftableDisplayModel> Draftables { get; }

        [JsonConstructor]
        public LineupModel(
            decimal salary,
            decimal fppg,
            decimal projectedFppg,
            List<DraftableDisplayModel> draftables)
        {
            Salary = salary;
            Fppg = fppg;
            ProjectedFppg = projectedFppg;
            Draftables = draftables;
        }

        public LineupModel(IEnumerable<DraftableDisplayModel> draftables)
        {
            Draftables = draftables.ToList();

            foreach (var draftable in draftables)
            {
                Salary += draftable.Salary;
                Fppg += draftable.Fppg;
                ProjectedFppg += draftable.ProjectedFppg;
            }
        }
    }
}
