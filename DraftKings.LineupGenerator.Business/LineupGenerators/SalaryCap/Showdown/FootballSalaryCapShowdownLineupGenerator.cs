using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;
using DraftKings.LineupGenerator.Models.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Filters;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    public class FootballSalaryCapShowdownLineupGenerator : BaseLineupGenerator<InitializedDraftableModel>
    {
        protected override string Description => "Salary Cap Showdown (Football)";

        public FootballSalaryCapShowdownLineupGenerator(
            IShowdownLineupService showdownLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(showdownLineupService, incrementalLogger,
                  new ProjectedPointsLineupsBag("FPPG"),
                  new FootballWeightedProjectedPointsLineupsBag("Opponent Weighted FPPG"))
        {

        }

        public override bool CanGenerate(ContestModel contest, RulesModel rules)
        {
            if (contest.ContestDetail.Sport != Sports.Nfl &&
                contest.ContestDetail.Sport != Sports.Xfl &&
                contest.ContestDetail.Sport != Sports.Cfb)
            {
                return false;
            }

            if (rules.DraftType != DraftTypes.SalaryCap || !rules.SalaryCap.IsEnabled)
            {
                return false;
            }

            return
                rules.GameTypeName == GameTypes.Showdown ||
                rules.GameTypeName == GameTypes.NflShowdown ||
                rules.GameTypeName == GameTypes.XflShowdown ||
                rules.GameTypeName == GameTypes.MaddenShowdown;
        }

        protected override bool IsValidLineup(LineupRequestModel request, RulesModel rules, DraftablesModel draftables, LineupModel lineup)
        {
            if (!base.IsValidLineup(request, rules, draftables, lineup))
            {
                return false;
            }

            if (request.PlayerRequests?.CaptainPlayerNameRequests == null ||
                request.PlayerRequests.CaptainPlayerNameRequests.Count == 0)
            {
                return true;
            }

            var captainRequests = request.PlayerRequests.CaptainPlayerNameRequests;

            foreach (var player in lineup.Draftables.Where(x => x.RosterPosition == RosterSlots.Captain))
            {
                if (captainRequests.Contains(player.Name))
                {
                    return true;
                }

                if (captainRequests.Contains(player.FirstName))
                {
                    return true;
                }

                if (captainRequests.Contains(player.LastName))
                {
                    return true;
                }
            }

            return false;
        }

        protected override List<InitializedDraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
        {
            var eligiblePlayers = draftables.Draftables
                .ExcludeOut()
                .ExcludeDisabled()
                .ExcludeZeroSalary()
                .ExcludeDoubtful()
                .ExcludeInjuredReserve()
                .ApplyRequestExclusions(request, rules)
                .ExcludeZeroFppg(draftables.DraftStats);

            if (!request.IncludeQuestionable)
            {
                eligiblePlayers = eligiblePlayers.ExcludeQuestionable();
            }

            if (!request.IncludeBaseSalary)
            {
                eligiblePlayers = eligiblePlayers.ExcludeBaseSalary();
            }

            if (request.ExcludeDefense)
            {
                eligiblePlayers = eligiblePlayers.Where(x => x.Position != RosterSlots.Dst);
            }

            if (request.ExcludeKickers)
            {
                eligiblePlayers = eligiblePlayers.Where(x => x.Position != RosterSlots.Kicker);
            }

            return eligiblePlayers
                .Select(x => new InitializedDraftableModel(x, rules, draftables))
                .ToList();
        }
    }
}
