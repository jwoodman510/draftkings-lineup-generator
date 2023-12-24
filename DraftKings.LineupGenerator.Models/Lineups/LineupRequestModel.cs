using System;
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

        public bool IncludeQuestionable { get; set; } = false;

        public bool IncludeBaseSalary { get; set; } = false;

        public decimal MinFppg { get; set; } = 10.0m;

        public bool ExcludeDefense { get; set; } = false;

        public bool ExcludeKickers { get; set; } = false;

        public string OutputFormat { get; set; } = "text";

        public int LineupCount { get; set; } = 5;

        public PlayerRequestsModel PlayerRequests { get; set; }

        public Guid? CorrelationId { get; set; }
    }
}
