using DraftKings.LineupGenerator.Models.Constants;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Comparers
{
    public class RosterPositionComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xOrdinal = GetOrdinal(x);
            var yOrdinal = GetOrdinal(y);

            if (xOrdinal == yOrdinal)
            {
                return 0;
            }

            return xOrdinal < yOrdinal ? -1 : 1;
        }

        private static int GetOrdinal(string position) => position switch
        {
            RosterSlots.Captain => 0,
            RosterSlots.Quarterback => 1,
            RosterSlots.RunningBack => 2,
            RosterSlots.WideReceiver => 3,
            RosterSlots.Flex => 4,
            RosterSlots.TightEnd => 5,
            RosterSlots.Dst => 6,
            RosterSlots.Kicker => 7,
            _ => int.MaxValue,
        };
    }
}
