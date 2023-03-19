using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupsModel
    {
        public string Description { get; set; }

        public List<LineupModel> Lineups { get; set; } = new List<LineupModel>();
    }
}
