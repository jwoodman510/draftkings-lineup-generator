using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap classic contests based on FPPG.
    /// Currently only supports NFL and XFL game types.
    /// </summary>
    public class DefaultSalaryCapClassicLineupGenerator : ILineupGenerator
    {
        private readonly IClassicLineupService _classicLineupService;

        public DefaultSalaryCapClassicLineupGenerator(IClassicLineupService classicLineupService)
        {
            _classicLineupService = classicLineupService;
        }

        public bool CanGenerate(RulesModel rules)
        {
            if (rules.DraftType != DraftTypes.SalaryCap || !rules.SalaryCap.IsEnabled)
            {
                return false;
            }

            return rules.GameTypeName == GameTypes.NflClassic || rules.GameTypeName == GameTypes.XflClassic;
        }

        public async Task<LineupsModel> GenerateAsync(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
        {
            await Task.Yield();

            var result = new LineupsModel
            {
                Description = "Default FPPG Lineup"
            };

            if (draftables.Draftables.All(x => x.Salary == default))
            {
                return result;
            }

            var eligiblePlayers = GetEligiblePlayers(request, draftables);

            var possibleLineups = _classicLineupService.GetPotentialLineups(rules, draftables, eligiblePlayers.ToList());

            result.Lineups = possibleLineups
                .Select(lineup => new LineupModel
                {
                    Draftables = lineup
                        .Select(player => new DraftableDisplayModel(
                            player.DisplayName,
                            player.GetFppg(draftables.DraftStats),
                            player.Salary,
                            player.GetRosterPosition(rules)))
                })
                .Where(x => x.Salary <= rules.SalaryCap.MaxValue && x.Salary >= rules.SalaryCap.MinValue)
                .GroupBy(x => x.Fppg)
                .OrderByDescending(x => x.Key)
                .First();

            return result;
        }

        private static List<DraftableModel> GetEligiblePlayers(LineupRequestModel request, DraftablesModel draftables)
        {
            var eligiblePlayers = draftables.Draftables
                .ExcludeOut()
                .ExcludeDisabled()
                .ExcludeZeroSalary()
                .ExcludeInjuredReserve()
                .ExcludeZeroSalary()
                .ExcludeZeroFppg(draftables.DraftStats);

            if (!request.IncludeQuestionable)
            {
                eligiblePlayers.ExcludeQuestionable();
            }

            return eligiblePlayers.ToList();
        }
    }
}
