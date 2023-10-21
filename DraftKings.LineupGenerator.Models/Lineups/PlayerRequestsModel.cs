using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class PlayerRequestsModel
    {
        public HashSet<string> PlayerNameRequests { get; set; }
        public HashSet<string> CaptainPlayerNameRequests { get; set; }
        public HashSet<string> PlayerNameExclusionRequests { get; set; }
        public HashSet<string> CaptainPlayerNameExclusionRequests { get; set; }
    }
}
