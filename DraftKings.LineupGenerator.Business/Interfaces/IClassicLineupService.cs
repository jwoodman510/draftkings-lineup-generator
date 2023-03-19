using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IClassicLineupService
    {
        IEnumerable<IEnumerable<DraftableModel>> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables);

        IEnumerable<IEnumerable<DraftableModel>> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables,
            IEnumerable<DraftableModel> eligiblePlayers);
    }
}
