using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using System;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupBags
{
    public class FootballWeightedProjectedPointsLineupsBag : ProjectedPointsLineupsBag
    {
        public FootballWeightedProjectedPointsLineupsBag(string description) : base(description)
        {

        }

        public override void UpdateLineups(ContestModel contest, LineupModel lineup, int max)
        {
            var updatedLineup = GetUpdatedLineup(contest, lineup);

            base.UpdateLineups(contest, updatedLineup, max);
        }

        private LineupModel GetUpdatedLineup(ContestModel contest, LineupModel lineupModel) => new LineupModel
        {
            Description = lineupModel.Description,
            Draftables = lineupModel.Draftables.Select(player =>
            {
                if (player.OpponentRank <= 0)
                {
                    return player;
                }

                switch (contest.ContestDetail.Sport)
                {
                    case Sports.Nfl:
                        return ModifyNflPlayer(player);
                    case Sports.Xfl:
                        return ModifyXflPlayer(player);
                    case Sports.Cfb:
                        return ModifyCfbPlayer(player);
                    default:
                        return player;
                }
            }).ToList()
        };

        private static DraftableDisplayModel ModifyNflPlayer(DraftableDisplayModel player)
        {
            // max multiplier: 8
            if (player.OpponentRank > 16)
            {
                var updatedFppg = player.ProjectedFppg + player.OpponentRank * 0.05m;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }
            else
            {
                var updatedFppg = player.ProjectedFppg - (17 - player.OpponentRank) * 0.05m;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }
        }

        private static DraftableDisplayModel ModifyXflPlayer(DraftableDisplayModel player)
        {
            if (player.OpponentRank > 5)
            {
                var updatedFppg = player.ProjectedFppg + player.OpponentRank;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }
            else if (player.OpponentRank < 4)
            {
                var updatedFppg = player.ProjectedFppg - player.OpponentRank;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }

            return player;
        }

        private static DraftableDisplayModel ModifyCfbPlayer(DraftableDisplayModel player)
        {
            if (player.OpponentRank >= 50)
            {
                var bonus = player.OpponentRank / 10;

                var updatedFppg = player.ProjectedFppg + bonus * 0.05m;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }

            if (player.OpponentRank <= 10)
            {
                var updatedFppg = player.ProjectedFppg - player.OpponentRank * 0.05m;

                return player.WithWeightedProjectedFppg(updatedFppg);
            }

            return player;
        }
    }
}
