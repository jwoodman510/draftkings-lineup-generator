namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public string Name { get; set; }

        public decimal Fppg { get; set; }

        public decimal ProjectedFppg { get; set; }

        public decimal Salary { get; set; }

        public string RosterPosition { get; set; }

        public int RosterPositionSortOrdinal => RosterPosition switch
        {
            "QB" => 0,
            "RB" => 1,
            "WR" => 2,
            "TE" => 4,
            "FLEX" => 5,
            "DST" => 6,
            _ => int.MaxValue,
        };

        public DraftableDisplayModel(string name, decimal fppg, decimal salary, string rosterPosition)
        {
            Name = name;
            Fppg = fppg;
            Salary = salary;
            ProjectedFppg = fppg;
            RosterPosition = rosterPosition;
        }

        public DraftableDisplayModel(string name, decimal fppg, decimal salary, string rosterPosition, decimal projectedFppg)
        {
            Name = name;
            Fppg = fppg;
            Salary = salary;
            ProjectedFppg = projectedFppg;
            RosterPosition = rosterPosition;
        }
    }
}
