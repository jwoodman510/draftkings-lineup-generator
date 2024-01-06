using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;

namespace DraftKings.LineupGenerator.Razor.Contests
{
    public class ContestHistoryModel
    {
        public Guid Id { get; set; }

        public ContestModel ContestModel { get; set; }

        public LineupRequestModel RequestModel { get; set; }

        public List<LineupsModel> Results { get; set; }
    }
}
