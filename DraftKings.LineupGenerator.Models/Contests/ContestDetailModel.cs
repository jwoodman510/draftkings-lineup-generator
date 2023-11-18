using System;

namespace DraftKings.LineupGenerator.Models.Contests
{
    public class ContestDetailModel
    {
        public int DraftGroupId { get; set; }

        public int GameTypeId { get; set; }

        public string Sport { get; set; }

        public string Name { get; set; }

        public DateTime ContestStartTime { get; set; }

        public string ContestSummary { get; set; }

        public decimal EntryFee { get; set; }

        public int Entries { get; set; }

        public int MaxEntries { get; set; }
    }
}
