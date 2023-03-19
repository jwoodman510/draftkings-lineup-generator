using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using System.Linq;

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
    }
}
