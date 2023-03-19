using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Services
{
    public class ClassicLineupService : IClassicLineupService
    {
        public IEnumerable<IEnumerable<DraftableModel>> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables) => GetAllPossibleLineups(rules, draftables, draftables.Draftables);

        public IEnumerable<IEnumerable<DraftableModel>> GetAllPossibleLineups(
            RulesModel rules,
            DraftablesModel draftables,
            IEnumerable<DraftableModel> eligiblePlayers) => GetPermutations(rules, eligiblePlayers);

        private static IEnumerable<IEnumerable<DraftableModel>> GetPermutations(RulesModel rules, IEnumerable<DraftableModel> eligiblePlayers)
        {
            var rosterSlotGroups = rules.LineupTemplate
                .GroupBy(x => x.RosterSlot.Id)
                .Select(x => new { RosterSlotId = x.Key, Count = x.Count() })
                .GroupBy(x => x.Count)
                .OrderBy(x => x.Key);

            var permutations = Enumerable.Empty<IEnumerable<DraftableModel>>();

            foreach (var rosterSlotGroup in rosterSlotGroups)
            {
                var slotCount = rosterSlotGroup.Key;
                var rosterSlotIds = rosterSlotGroup.Select(x => x.RosterSlotId).ToHashSet();

                var rosterSlotPlayers = eligiblePlayers.Where(x => rosterSlotIds.Contains(x.RosterSlotId));
                var rosterSlotPermutations = GetPermutationsForSlots(rosterSlotIds, rosterSlotPlayers, slotCount);

                permutations = permutations.CombinePermutations(rosterSlotPermutations, filterPredicate: ShouldCombinePermutations);
            }

            return permutations;
        }

        private static IEnumerable<IEnumerable<DraftableModel>> GetPermutationsForSlots(HashSet<int> rosterSlotIds, IEnumerable<DraftableModel> slotPlayers, int slotCount)
        {
            var rosterSlotPermutations = rosterSlotIds.Select(x => slotPlayers.Where(p => p.RosterSlotId == x).GetPermutations(slotCount));

            return rosterSlotPermutations.Aggregate(Enumerable.Empty<IEnumerable<DraftableModel>>(), (rosterSlotPermutation, aggregate) =>
            {
                return rosterSlotPermutation.CombinePermutations(aggregate, filterPredicate: ShouldCombinePermutations);
            });
        }

        private static bool ShouldCombinePermutations(IEnumerable<DraftableModel> first, IEnumerable<DraftableModel> second)
        {
            var secondPlayerIds = second.Select(x => x.PlayerId).ToHashSet();

            var hasDuplicate = first.Select(x => x.PlayerId).Any(x => secondPlayerIds.Contains(x));

            return !hasDuplicate;
        }
    }
}
