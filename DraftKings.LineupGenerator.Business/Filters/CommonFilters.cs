using DraftKings.LineupGenerator.Models.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class CommonFilters
    {
        public static IEnumerable<DraftableModel> ExcludeDisabled(this IEnumerable<DraftableModel> draftables) =>
            draftables.Where(x => !x.IsDisabled);

        public static IEnumerable<DraftableModel> ApplyRequestExclusions(this IEnumerable<DraftableModel> draftables, LineupRequestModel request, RulesModel rules)
        {
            if (request.PlayerRequests?.PlayerNameExclusionRequests?.Count > 0)
            {
                draftables = draftables
                    .Where(x => !request.PlayerRequests.PlayerNameExclusionRequests.Contains(x.FirstName))
                    .Where(x => !request.PlayerRequests.PlayerNameExclusionRequests.Contains(x.LastName))
                    .Where(x => !request.PlayerRequests.PlayerNameExclusionRequests.Contains(x.DisplayName));
            }

            if (request.PlayerRequests?.CaptainPlayerNameExclusionRequests?.Count > 0)
            {
                var captainSlot = rules.LineupTemplate.FirstOrDefault(x => x.RosterSlot.Name == RosterSlots.Captain)?.RosterSlot?.Id;

                if (captainSlot != null)
                {
                    draftables = draftables
                        .Where(x => x.RosterSlotId != captainSlot || !request.PlayerRequests.CaptainPlayerNameExclusionRequests.Contains(x.FirstName))
                        .Where(x => x.RosterSlotId != captainSlot || !request.PlayerRequests.CaptainPlayerNameExclusionRequests.Contains(x.LastName))
                        .Where(x => x.RosterSlotId != captainSlot || !request.PlayerRequests.CaptainPlayerNameExclusionRequests.Contains(x.DisplayName));
                }                
            }

            return draftables;
        }
    }
}
