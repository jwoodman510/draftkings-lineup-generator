using DraftKings.LineupGenerator.Models.Extensions;
using DraftKings.LineupGenerator.Models.Rules;

namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class InitializedDraftableModel : DraftableModel
    {
        public decimal Fppg { get; }

        public string RosterPosition { get; }

        public decimal ProjectedSalary { get; }

        public int OpponentRank { get; }

        public InitializedDraftableModel(DraftableModel draftableModel, RulesModel rules, DraftablesModel draftables)
        {
            DraftableId = draftableModel.DraftableId;
            RosterSlotId = draftableModel.RosterSlotId;
            Salary = draftableModel.Salary;
            IsDisabled = draftableModel.IsDisabled;
            Position = draftableModel.Position;
            Status = draftableModel.Status;
            TeamId = draftableModel.TeamId;
            PlayerId = draftableModel.PlayerId;
            DisplayName = draftableModel.DisplayName;
            FirstName = draftableModel.FirstName;
            LastName = draftableModel.LastName;
            DraftStatAttributes = draftableModel.DraftStatAttributes;

            Fppg = this.GetFppg(draftables.DraftStats);
            RosterPosition = this.GetRosterPosition(rules);
            ProjectedSalary = this.GetProjectedSalary(draftables, rules);
            OpponentRank = this.GetOpponentRank(draftables.DraftStats);
        }

        public override DraftableDisplayModel ToDisplayModel(RulesModel rules, DraftablesModel draftables)
        {
            return new DraftableDisplayModel(
                PlayerId,
                DisplayName,
                FirstName,
                LastName,
                this.GetFppg(draftables.DraftStats),
                Salary,
                this.GetRosterPosition(rules),
                this.GetProjectedSalary(draftables, rules),
                this.GetOpponentRank(draftables.DraftStats));
        }
    }
}
