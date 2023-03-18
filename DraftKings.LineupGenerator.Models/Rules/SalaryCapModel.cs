namespace DraftKings.LineupGenerator.Models.Rules
{
    public class SalaryCapModel
    {
        public bool IsEnabled { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}
