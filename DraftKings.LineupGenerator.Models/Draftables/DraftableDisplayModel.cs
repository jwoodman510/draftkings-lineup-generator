namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public string Name { get; set; }

        public decimal FPPG { get; set; }

        public string Position { get; set; }

        public int Salary { get; set; }

        public DraftableDisplayModel(string name, string position, decimal fppg, int salary)
        {
            Name = name;
            FPPG = fppg;
            Position = position;
            Salary = salary;
        }
    }
}
