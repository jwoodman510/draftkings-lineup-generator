using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupModel
    {
        public string Description { get; set; }

        public decimal Salary => Draftables.Sum(x => x.Salary);

        public decimal Fppg => Draftables.Sum(x => x.Fppg);

        public List<DraftableDisplayModel> Draftables { get; set; }
    }
}
