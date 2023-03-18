using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupModel
    {
        public string Description { get; set; }

        public int Salary { get; set; }

        public List<DraftableDisplayModel> Draftables { get; set; }
    }
}
