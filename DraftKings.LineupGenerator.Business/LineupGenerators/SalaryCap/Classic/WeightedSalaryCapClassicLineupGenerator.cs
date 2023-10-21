using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The weighted lineup generator for salary cap classic contests based on FPPG with an opponent ranking multiplier
    /// Currently only supports CFB, Madden, NFL, and XFL game types.
    /// </summary>
    public class WeightedSalaryCapClassicLineupGenerator : BaseSalaryCapClassicLineupGenerator
    {
        public WeightedSalaryCapClassicLineupGenerator(
            IClassicLineupService classicLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(new ProjectedPointsLineupsBag("Opponent Weighted FPPG"), classicLineupService, incrementalLogger)
        {

        }

        protected override void ModifyLineup(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, LineupModel lineup)
        {
            foreach (var player in lineup.Draftables)
            {
                if (player.OpponentRank <= 0)
                {
                    return;
                }

                switch (contest.ContestDetail.Sport)
                {
                    case Sports.Nfl:
                        ModifyNflPlayer(player);
                        break;
                    case Sports.Xfl:
                        ModifyXflPlayer(player);
                        break;
                    case Sports.Cfb:
                        ModifyCfbPlayer(player);
                        break;
                }
            }
        }

        private static void ModifyNflPlayer(DraftableDisplayModel player)
        {
            // max multiplier: 8
            if (player.OpponentRank > 16)
            {
                player.ProjectedFppg += player.OpponentRank * 0.05m;
            }
            else
            {
                player.ProjectedFppg -= (17 - player.OpponentRank) * 0.05m;
            }
        }

        private static void ModifyXflPlayer(DraftableDisplayModel player)
        {
            if (player.OpponentRank > 5)
            {
                player.ProjectedFppg += player.OpponentRank;
            }
            else if (player.OpponentRank < 4)
            {
                player.ProjectedFppg -= player.OpponentRank;
            }
        }

        private static void ModifyCfbPlayer(DraftableDisplayModel player)
        {
            if (player.OpponentRank >= 50)
            {
                var bonus = player.OpponentRank / 10;

                player.ProjectedFppg += bonus * 0.05m;

                return;
            }

            if (player.OpponentRank <= 10)
            {
                player.ProjectedFppg -= player.OpponentRank * 0.05m;
            }
        }
    }
}
