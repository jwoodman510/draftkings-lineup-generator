using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Comparers
{
    public class DraftableRosterPositionComparer : IComparer<DraftableDisplayModel>
    {
        public int Compare(DraftableDisplayModel x, DraftableDisplayModel y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return new RosterPositionComparer().Compare(x.RosterPosition, y.RosterPosition);
        }
    }
}
