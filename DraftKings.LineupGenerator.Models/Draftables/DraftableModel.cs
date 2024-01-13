using DraftKings.LineupGenerator.Models.Extensions;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableModel
    {
        public int DraftableId { get; set; }

        public int RosterSlotId { get; set; }

        public int Salary { get; set; }

        public bool IsDisabled { get; set; }

        public string Position { get; set; }

        public string Status { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<DraftStatAttributeModel> DraftStatAttributes { get; set; }

        public virtual DraftableDisplayModel ToDisplayModel(RulesModel rules, DraftablesModel draftables)
        {
            return new DraftableDisplayModel(
                PlayerId,
                DisplayName,
                FirstName,
                LastName,
                this.GetFppg(draftables.DraftStats),
                Salary,
                Position,
                this.GetRosterPosition(rules),
                this.GetProjectedSalary(draftables, rules),
                this.GetOpponentRank(draftables.DraftStats));
        }
    }
}
