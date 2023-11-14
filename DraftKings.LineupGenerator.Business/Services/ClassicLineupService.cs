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
        public IEnumerable<IEnumerable<T>> GetPotentialLineups<T>(
            LineupRequestModel request,
            RulesModel rules,
            DraftablesModel draftables,
            List<T> eligiblePlayers) where T : DraftableModel
        {
            if (request.PlayerRequests?.PlayerNameRequests?.Count > 0)
            {
                return GetPotentialLineupsWithPlayerRequests(request.PlayerRequests.PlayerNameRequests, rules, eligiblePlayers);
            }

            return GetAllPotentialLineups(rules, eligiblePlayers);
        }

        public IEnumerable<IEnumerable<T>> GetPotentialLineupsWithPlayerRequests<T>(
            HashSet<string> playerRequests,
            RulesModel rules,
            List<T> eligiblePlayers) where T : DraftableModel
        {
            // This is meant to speed things up for a player request with a single roster slot.
            var singleRosterSlots = rules.LineupTemplate
                .GroupBy(x => x.RosterSlot.Id)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key)
                .ToHashSet();

            if (!singleRosterSlots.Any())
            {
                return GetAllPotentialLineups(rules, eligiblePlayers);
            }

            var singleSlotRequestedPlayers = eligiblePlayers
                .Where(x => singleRosterSlots.Contains(x.RosterSlotId))
                .Where(x => playerRequests.Contains(x.FirstName) ||
                        playerRequests.Contains(x.LastName) ||
                        playerRequests.Contains(x.DisplayName))
                .ToList();

            var matchedSingleRosterSlots = singleSlotRequestedPlayers.Select(x => x.RosterSlotId).ToHashSet();

            var remainingPlayers = eligiblePlayers.Except(singleSlotRequestedPlayers).ToList();

            var remainingSlotGroups = rules.LineupTemplate
                .GroupBy(x => x.RosterSlot.Id)
                .Where(x => !matchedSingleRosterSlots.Contains(x.Key))
                .Select(x => new { RosterSlotId = x.Key, Count = x.Count() })
                .GroupBy(x => x.Count)
                .OrderBy(x => x.Key);

            var permutations = Enumerable.Empty<IEnumerable<T>>();

            foreach (var rosterSlotGroup in remainingSlotGroups)
            {
                var slotCount = rosterSlotGroup.Key;
                var rosterSlotIds = rosterSlotGroup.Select(x => x.RosterSlotId).ToHashSet();

                var rosterSlotPlayers = remainingPlayers.Where(x => rosterSlotIds.Contains(x.RosterSlotId));
                var rosterSlotPermutations = GetPermutationsForSlots(rosterSlotIds, rosterSlotPlayers, slotCount);

                permutations = permutations
                    .CombinePermutations(rosterSlotPermutations)
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));
            }

            permutations = permutations
                    .CombinePermutations(new[] { singleSlotRequestedPlayers })
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));

            return permutations;
        }

        private static IEnumerable<IEnumerable<T>> GetAllPotentialLineups<T>(RulesModel rules, List<T> eligiblePlayers) where T : DraftableModel
        {
            var rosterSlotGroups = rules.LineupTemplate
                .GroupBy(x => x.RosterSlot.Id)
                .Select(x => new { RosterSlotId = x.Key, Count = x.Count() })
                .GroupBy(x => x.Count)
                .OrderBy(x => x.Key);

            var permutations = Enumerable.Empty<IEnumerable<T>>();

            foreach (var rosterSlotGroup in rosterSlotGroups)
            {
                var slotCount = rosterSlotGroup.Key;
                var rosterSlotIds = rosterSlotGroup.Select(x => x.RosterSlotId).ToHashSet();

                var rosterSlotPlayers = eligiblePlayers.Where(x => rosterSlotIds.Contains(x.RosterSlotId));
                var rosterSlotPermutations = GetPermutationsForSlots(rosterSlotIds, rosterSlotPlayers, slotCount);

                permutations = permutations
                    .CombinePermutations(rosterSlotPermutations)
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));
            }

            return permutations;
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsForSlots<T>(HashSet<int> rosterSlotIds, IEnumerable<T> slotPlayers, int slotCount)
             where T : DraftableModel
        {
            var rosterSlotPermutations = rosterSlotIds
                .Select(x => slotPlayers.Where(p => p.RosterSlotId == x)
                .GetPermutations(slotCount));

            return rosterSlotPermutations.Aggregate(Enumerable.Empty<IEnumerable<T>>(), (rosterSlotPermutation, aggregate) =>
            {
                return rosterSlotPermutation
                    .CombinePermutations(aggregate)
                    .Where(x => x.DistinctBy(y => y.PlayerId).SequenceEqual(x));
            });
        }
    }
}
