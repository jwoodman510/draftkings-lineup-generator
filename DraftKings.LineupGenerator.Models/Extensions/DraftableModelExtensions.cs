using DraftKings.LineupGenerator.Models.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Models.Extensions
{
    public static class DraftableModelExtensions
    {
        public static DraftStatAttributeModel GetDraftStatAttribute(this DraftableModel player, DraftStatModel draftStat)
        {
            if (draftStat == null)
            {
                return null;
            }

            return player.DraftStatAttributes.FirstOrDefault(x => x.Id == draftStat.Id);
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

            return decimal.TryParse(stat.Value, out var val) ? val : default;
        }

        public static int GetOpponentRank(this DraftableModel player, List<DraftStatModel> draftStats)
        {
            var oppRankStat = draftStats.Single(x => x.Name == DraftStats.OpponentRank);

            var stat = player.GetDraftStatAttribute(oppRankStat);

            return int.TryParse(stat.SortValue, out var val) ? val : default;
        }

        public static decimal GetProjectedSalary(this DraftableModel player, DraftablesModel draftables, RulesModel rules)
        {
            var captainSlot = rules.LineupTemplate.FirstOrDefault(x => x.RosterSlot.Name == RosterSlots.Captain);

            var projectedFppg = player.GetFppg(draftables.DraftStats);

            return captainSlot != null && player.RosterSlotId == captainSlot.RosterSlot.Id
                ? projectedFppg * 1.5m
                : projectedFppg;
        }
    }
}
