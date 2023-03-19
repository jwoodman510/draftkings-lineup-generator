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

            var eligiblePlayers = draftables.Draftables
                .ExcludeOut()
                .ExcludeDisabled()
                .ExcludeZeroSalary()
                .ExcludeInjuredReserve()
                .ExcludeZeroSalary();

            if (!request.IncludeQuestionable)
            {
                eligiblePlayers.ExcludeQuestionable();
            }

            var possibleLineups = _classicLineupService.GetAllPossibleLineups(rules, draftables, eligiblePlayers);

            result.Lineups = possibleLineups
                .Where(x => x.Salary <= rules.SalaryCap.MaxValue)
                .Where(x => x.Salary >= rules.SalaryCap.MinValue)
                .GroupBy(x => x.Salary)
                .OrderByDescending(x => x.Key)
                .FirstOrDefault()
                ?.ToList() ?? new List<LineupModel>();

            return result;
        }
    }
}
