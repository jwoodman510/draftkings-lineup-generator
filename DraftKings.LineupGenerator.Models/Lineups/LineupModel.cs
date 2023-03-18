using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupModel
    {
        public string Description { get; set; }

        public List<DraftablesModel> Draftables { get; set; }
    }
}
