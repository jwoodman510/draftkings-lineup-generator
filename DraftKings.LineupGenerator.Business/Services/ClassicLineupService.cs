using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Services
{
    public class ClassicLineupService : IClassicLineupService
    {
        public IEnumerable<LineupModel> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables)
        {
            return GetAllPossibleLineups(rules, draftables, draftables.Draftables);
        }

        public IEnumerable<LineupModel> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables,
            IEnumerable<DraftableModel> eligiblePlayers)
        {
            var salaryCap = rules.SalaryCap.MaxValue;
            var fppgDraftStat = draftables.DraftStats.Single(x => x.Name == DraftStats.FantasyPointsPerGame);

            var permuations = GetPermutations(rules, eligiblePlayers);

            var validTemplateRosterIds = rules.LineupTemplate
                .Select(x => x.RosterSlot.Id)
                .OrderBy(x => x)
                .ToList();

            var validPermutations = permuations.Where(x =>
                x.Select(p => p.RosterSlotId)
                .OrderBy(id => id)
                .SequenceEqual(validTemplateRosterIds));

            return validPermutations.Select(permutation => new LineupModel
            {
                Draftables = permutation
                    .Select(player => new DraftableDisplayModel(
                        player.DisplayName,
                        player.GetDraftStatAttribute(fppgDraftStat),
                        player.Salary,
                        player.GetRosterPosition(rules)))
                    .ToList()
            });
        }

        private static IEnumerable<IEnumerable<DraftableModel>> GetPermutations(RulesModel rules, IEnumerable<DraftableModel> eligiblePlayers)
        {
            var singleRosterSlots = rules.LineupTemplate
                .GroupBy(x => x.RosterSlot.Id)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key)
                .ToHashSet();

            if (singleRosterSlots.Count == 0)
            {
                return eligiblePlayers.GetPermutations(rules.LineupTemplate.Count);
            }

            var permutations = Enumerable.Empty<IEnumerable<DraftableModel>>();

            var singleSlotPlayers = eligiblePlayers
                .Where(x => singleRosterSlots.Contains(x.RosterSlotId))
                .ToList();

            var nonSingleSlotPlayers = eligiblePlayers
                .Where(x => !singleRosterSlots.Contains(x.RosterSlotId))
                .ToList();

            var singleSlotPermutations = singleSlotPlayers.GetPermutations(singleSlotPlayers.Count);
            var nonSingleSlotPermutations = nonSingleSlotPlayers.GetPermutations(rules.LineupTemplate.Count - singleRosterSlots.Count);

            var second = nonSingleSlotPermutations.ToList();
            var first = singleSlotPermutations.ToList();
            var combined = first.CombinePermutations(second).ToList();

            return singleSlotPermutations.CombinePermutations(nonSingleSlotPermutations);
        }
    }
}
