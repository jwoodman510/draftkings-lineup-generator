using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;

namespace DraftKings.LineupGenerator.Razor.Contests
{
    public class ContestHistoryModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        public ContestModel ContestModel { get; set; } = default!;

        public LineupRequestModel RequestModel { get; set; } = default!;

        public List<LineupsModel> Results { get; set; } = default!;
    }
}
