namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableDisplayModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Fppg { get; set; }

        public decimal ProjectedFppg { get; set; }

        public decimal Salary { get; set; }

        public string RosterPosition { get; set; }

        public int OpponentRank { get; set; }

        public DraftableDisplayModel(
            long id,
            string name,
            string firstName,
            string lastName,
            decimal fppg,
            decimal salary,
            string rosterPosition,
            decimal projectedFppg,
            int opponentRank)
        {
            Id = id;
            Name = name;
            FirstName = firstName;
            LastName = lastName;
            Fppg = fppg;
            Salary = salary;
            ProjectedFppg = projectedFppg;
            RosterPosition = rosterPosition;
            OpponentRank = opponentRank;
        }

        public DraftableDisplayModel WithWeightedProjectedFppg(decimal projectedFppg)
        {
            return new DraftableDisplayModel(
                Id,
                Name,
                FirstName,
                LastName,
                Fppg,
                Salary,
                RosterPosition,
                projectedFppg,
                OpponentRank);
        }
    }
}
