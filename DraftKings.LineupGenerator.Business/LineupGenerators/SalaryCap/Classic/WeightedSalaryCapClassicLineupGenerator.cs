using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The weighted lineup generator for salary cap classic contests based on FPPG with an opponent ranking multiplier
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class WeightedSalaryCapClassicLineupGenerator : BaseSalaryCapClassicLineupGenerator
    {
        public WeightedSalaryCapClassicLineupGenerator(
            IClassicLineupService classicLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(new ProjectedPointsLineupsBag("Opponent Weighted FPPG"), classicLineupService, incrementalLogger)
        {

        }

        protected override void ModifyLineup(LineupRequestModel request, RulesModel rules, DraftablesModel draftables, LineupModel lineup)
        {
            foreach (var player in lineup.Draftables)
            {
                if (player.OpponentRank <= 0)
                {
                    continue;
                }

                if (player.OpponentRank > 16)
                {
                    // Bonus 0.5 points, max bonus of 16 if the rank is last
                    player.ProjectedFppg += player.OpponentRank * 0.5m;
                }
                else
                {
                    // Bonus -0.5 points, max bonus of -8 if the rank is first
                    player.ProjectedFppg -= (17 - player.OpponentRank) * 0.5m;
                }
            }
        }
    }
}
