using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IClassicLineupService
    {
        IEnumerable<LineupModel> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables);

        IEnumerable<LineupModel> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables,
            IEnumerable<DraftableModel> eligiblePlayers);
    }
}
