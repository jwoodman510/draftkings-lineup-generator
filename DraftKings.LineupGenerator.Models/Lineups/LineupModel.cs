using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupModel
    {
        public string Description { get; set; }

        public int Salary => Draftables?.Sum(x => x.Salary) ?? default;

        public IEnumerable<DraftableDisplayModel> Draftables { get; set; }
    }
}
