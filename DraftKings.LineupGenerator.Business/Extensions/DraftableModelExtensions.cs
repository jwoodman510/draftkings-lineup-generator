using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DraftKings.LineupGenerator.Business.Extensions
{
    public static class DraftableModelExtensions
    {
        public static string GetDraftStatAttribute(this DraftableModel player, DraftStatModel draftStat)
        {
            if (draftStat == null)
            {
                return null;
            }

            return player.DraftStatAttributes.FirstOrDefault(x => x.Id == draftStat.Id)?.Value;
        }

        public static string GetRosterPosition(this DraftableModel player, RulesModel rules)
        {
            return rules.LineupTemplate
                .Select(x => x.RosterSlot)
                .First(x => x.Id == player.RosterSlotId)
                .Name;
        }

        public static decimal GetFppg(this DraftableModel player, List<DraftStatModel> draftStats)
        {
            var fppgDraftStat = draftStats.Single(x => x.Name == DraftStats.FantasyPointsPerGame);

            var stat = player.GetDraftStatAttribute(fppgDraftStat);

            return decimal.TryParse(stat, out var val) ? val : default;
        }

        public static decimal GetProjectedSalary(this DraftableModel player, DraftablesModel draftables, RulesModel rules)
        {
            var captainSlot = rules.LineupTemplate.FirstOrDefault(x => x.RosterSlot.Name == RosterSlots.Captain);

            var projectedFppg = player.GetFppg(draftables.DraftStats);

            return captainSlot != null && player.RosterSlotId == captainSlot.RosterSlot.Id
                ? projectedFppg * 1.5m
                : projectedFppg;
        }

        public static DraftableDisplayModel ToDisplayModel(this DraftableModel player, RulesModel rules, DraftablesModel draftables)
        {
            return new DraftableDisplayModel(
                player.PlayerId,
                player.DisplayName,
                player.FirstName,
                player.LastName,
                player.GetFppg(draftables.DraftStats),
                player.Salary,
                player.GetRosterPosition(rules),
                player.GetProjectedSalary(draftables, rules));
        }
    }
}
