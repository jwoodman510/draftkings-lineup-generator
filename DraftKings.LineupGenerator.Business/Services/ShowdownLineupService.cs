using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Services
{
    public class ShowdownLineupService : IShowdownLineupService
    {
        public IEnumerable<IEnumerable<DraftableModel>> GetPotentialLineups(
            RulesModel rules,
            DraftablesModel draftables,
            List<DraftableModel> eligiblePlayers)
        {
            var teamIds = eligiblePlayers.Select(x => x.TeamId).Distinct().OrderBy(x => x).ToList();

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
                var rosterSlotPermutations = GetPermutationsForSlots(rosterSlotIds, rosterSlotPlayers, slotCount).ToList();

                permutations = permutations
                    .CombinePermutations(rosterSlotPermutations)
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));
            }

            // Showdown requires picking a player from each team
            return permutations.Where(x => x.Select(y => y.TeamId).Distinct().OrderBy(y => y).SequenceEqual(teamIds));
        }

        private static IEnumerable<IEnumerable<DraftableModel>> GetPermutationsForSlots(HashSet<int> rosterSlotIds, IEnumerable<DraftableModel> slotPlayers, int slotCount)
        {
            var rosterSlotPermutations = rosterSlotIds
                .Select(x => slotPlayers.Where(p => p.RosterSlotId == x)
                .GetPermutations(slotCount));

            return rosterSlotPermutations.Aggregate(Enumerable.Empty<IEnumerable<DraftableModel>>(), (rosterSlotPermutation, aggregate) =>
            {
                return rosterSlotPermutation
                    .CombinePermutations(aggregate)
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));
            });
        }
    }
}
