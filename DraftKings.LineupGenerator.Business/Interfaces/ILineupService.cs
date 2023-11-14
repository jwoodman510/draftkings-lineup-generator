using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupService
    {
        IEnumerable<IEnumerable<T>> GetPotentialLineups<T>(
            LineupRequestModel request,
            RulesModel rules,
            DraftablesModel draftables,
            List<T> eligiblePlayers) where T : DraftableModel;
    }
}
