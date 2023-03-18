using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap classic contests based on FPPG.
    /// Currently only supports NFL and XFL game types.
    /// </summary>
    public class DefaultSalaryCapClassicLineupGenerator : ILineupGenerator
    {
        public bool CanGenerate(RulesModel rules)
        {
            if (rules?.DraftType != DraftTypes.SalaryCap)
            {
                return false;
            }

            return rules.GameTypeName == GameTypes.NflClassic || rules.GameTypeName == GameTypes.XflClassic;
        }

        public async Task<LineupsModel> GenerateAsync(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
        {
            await Task.Yield();

            var filteredDraftables = draftables.Draftables
                .ExcludeOut()
                .ExcludeDisabled()
                .ExcludeZeroSalary()
                .ExcludeInjuredReserve()
                .ExcludeZeroSalary();

            if (!request.IncludeQuestionable)
            {
                filteredDraftables.ExcludeQuestionable();
            }

            var salaryCap = rules.SalaryCap.MaxValue;

            return new LineupsModel
            {
                Description = "Default FPPG Lineup",
                Lineups = new List<LineupModel>
                {
                    new LineupModel
                    {
                        Draftables = new List<DraftableDisplayModel>()
                    }
                }
            };
        }
    }
}
