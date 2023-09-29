using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupService
    {
        IEnumerable<IEnumerable<DraftableModel>> GetPotentialLineups(
            RulesModel rules,
            DraftablesModel draftables,
            List<DraftableModel> eligiblePlayers);
    }
}
