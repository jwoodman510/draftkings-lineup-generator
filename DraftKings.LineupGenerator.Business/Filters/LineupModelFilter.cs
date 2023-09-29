using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System;
using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Business.Filters
{
    public static class LineupModelFilter
    {
        public static bool MeetsSalaryCap(this LineupModel lineup, RulesModel rules)
        {
            if (!rules.SalaryCap.IsEnabled)
            {
                return true;
            }

            return lineup.Salary <= rules.SalaryCap.MaxValue && lineup.Salary >= rules.SalaryCap.MinValue;
        }

        public static bool IncludesPlayerRequests(this LineupModel lineup, PlayerRequestsModel playerRequests)
        {
            if (playerRequests == null || playerRequests.PlayerNameRequests == null || playerRequests.PlayerNameRequests.Count == 0)
            {
                return true;
            }

            var requests = new HashSet<string>(playerRequests.PlayerNameRequests, StringComparer.OrdinalIgnoreCase);

            foreach (var player in lineup.Draftables)
            {
                if (playerRequests.PlayerNameRequests.Contains(player.Name))
                {
                    requests.Remove(player.Name);
                }

                if (playerRequests.PlayerNameRequests.Contains(player.FirstName))
                {
                    requests.Remove(player.FirstName);
                }

                if (playerRequests.PlayerNameRequests.Contains(player.LastName))
                {
                    requests.Remove(player.LastName);
                }
            }

            return requests.Count == 0;
        }
    }
}
