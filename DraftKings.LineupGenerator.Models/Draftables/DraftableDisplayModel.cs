namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public string Name { get; set; }

        public decimal Fppg { get; set; }

        public decimal ProjectedFppg { get; set; }

        public decimal Salary { get; set; }

        public string RosterPosition { get; set; }

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
