using System.ComponentModel.DataAnnotations;

namespace DraftKings.LineupGenerator.Models.Lineups
{
    public class LineupRequestModel
    {
        public LineupRequestModel(int contestId)
        {
            ContestId = contestId;
        }

        [Required]
        [Range(1, int.MaxValue)]
        public int ContestId { get; init; }
    }
}
