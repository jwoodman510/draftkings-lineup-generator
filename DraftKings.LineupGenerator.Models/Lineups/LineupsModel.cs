using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupsModel
    {
        public string Description { get; set; }

        public IEnumerable<LineupModel> Lineups { get; set; }
    }
}
