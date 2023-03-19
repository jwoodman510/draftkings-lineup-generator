using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
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

            var fppgDraftStat = draftables.DraftStats.Single(x => x.Name == DraftStats.FantasyPointsPerGame);

            var count = possibleLineups.Count();

            result.Lineups = possibleLineups
                .Select(x => new { Lineup = x, Salary = x.Sum(y => y.Salary) })
                .Where(x => x.Salary <= rules.SalaryCap.MaxValue && x.Salary >= rules.SalaryCap.MinValue)
                .OrderByDescending(x => x.Salary)
                .Take(1)
                .Select(x => x.Lineup)
                .Select(lineup => new LineupModel
                {
                    Draftables = lineup
                        .Select(player => new DraftableDisplayModel(
                            player.DisplayName,
                            player.GetDraftStatAttribute(fppgDraftStat),
                            player.Salary,
                            player.GetRosterPosition(rules)))
                });

            return result;
        }
    }
}
