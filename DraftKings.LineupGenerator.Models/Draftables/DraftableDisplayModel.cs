namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public string Name { get; set; }

        public decimal Fppg { get; set; }

        public decimal Salary { get; set; }

        public string RosterPosition { get; set; }

        public DraftableDisplayModel(string name, decimal fppg, decimal salary, string rosterPosition)
        {
            Name = name;
            Fppg = fppg;
            Salary = salary;
            RosterPosition = rosterPosition;
        }
    }
}
