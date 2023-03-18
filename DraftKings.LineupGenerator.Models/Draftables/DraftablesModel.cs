using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftablesModel
    {
        public List<DraftableModel> Draftables { get; set; }

        public List<DraftStatModel> DraftStats { get; set; }
    }
}
