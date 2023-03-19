namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public string Name { get; set; }

        public string Fppg { get; set; }

        public int Salary { get; set; }

        public string RosterPosition { get; set; }

        public DraftableDisplayModel(string name, string fppg, int salary, string rosterPosition)
        {
            Name = name;
            Fppg = fppg;
            Salary = salary;
            RosterPosition = rosterPosition;
        }
    }
}
