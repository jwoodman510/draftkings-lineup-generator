namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftablesDisplayModel
    {
        public string Name { get; set; }

        public decimal FPPG { get; set; }

        public string Position { get; set; }

        public DraftablesDisplayModel(string name, string position, decimal fppg)
        {
            Name = name;
            FPPG = fppg;
            Position = position;
        }
    }
}
