using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    public abstract class BaseSalaryCapClassicLineupGenerator : BaseLineupGenerator
    {
        public BaseSalaryCapClassicLineupGenerator(
            BaseLineupsBag lineupsBag,
            IClassicLineupService classicLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(lineupsBag, classicLineupService, incrementalLogger)
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
                rules.GameTypeName == GameTypes.Classic ||
                rules.GameTypeName == GameTypes.NflClassic ||
                rules.GameTypeName == GameTypes.XflClassic ||
                rules.GameTypeName == GameTypes.MaddenClassic;
        }

        protected override List<DraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
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
                eligiblePlayers = eligiblePlayers.ExcludeBaseSalaryByPosition();
            }

            var dstRosterSlot = rules.LineupTemplate.SingleOrDefault(x => x.RosterSlot.Name == RosterSlots.Dst)?.RosterSlot;

            var remainingPlayers = eligiblePlayers
                .Where(x => x.RosterSlotId != dstRosterSlot?.Id)
                .Where(x => x.Position != RosterSlots.Quarterback)
                .MinimumFppg(draftables.DraftStats, request.MinFppg);

            var dstPlayers = eligiblePlayers.Where(x => x.RosterSlotId == dstRosterSlot?.Id);
            var quarterbacks = eligiblePlayers.Where(x => x.Position == RosterSlots.Quarterback);

            var highestSalaryQuarterbacksByTeam = quarterbacks.GroupBy(x => x.TeamId).Select(x => x.OrderByDescending(y => y.Salary).First());

            return remainingPlayers.Concat(dstPlayers).Concat(highestSalaryQuarterbacksByTeam)
                .OrderBy(x => x.Salary)
                .ToList();
        }
    }
}
