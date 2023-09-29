using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap showdown contests based on FPPG.
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapShowdownLineupGenerator : BaseLineupGenerator
    {
        protected override string Description => "Projected FPPG";

        public DefaultSalaryCapShowdownLineupGenerator(
            IShowdownLineupService showdownLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(new ProjectedPointsLineupsBag(), showdownLineupService, incrementalLogger)
        {

        }

        public override bool CanGenerate(ContestModel contest, RulesModel rules)
        {
            if (contest.ContestDetail.Sport != Sports.Nfl &&
                contest.ContestDetail.Sport != Sports.Xfl)
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

        protected override List<DraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
        {
            var eligiblePlayers = draftables.Draftables
                .ExcludeOut()
                .ExcludeDisabled()
                .ExcludeZeroSalary()
                .ExcludeDoubtful()
                .ExcludeInjuredReserve()
                .ExcludeZeroSalary()
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

            return eligiblePlayers.ToList();
        }
    }
}
