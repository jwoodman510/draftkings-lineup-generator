using DraftKings.LineupGenerator.Api;
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

        public async Task<LineupsModel> GenerateAsync(RulesModel rules, DraftablesModel draftables)
        {
            await Task.Yield();

            // TODO: The logic...

            // Remove Disabled
            // Remove 0 FPPG
            // Remove Inactive
            // Remove Questionable

            return new LineupsModel
            {
                Description = "Default FPPG Lineup (Excluding Questionables)",
                Lineups = new List<LineupModel>
                {
                    new LineupModel
                    {
                        Draftables = new List<DraftablesModel>()
                    }
                }
            };
        }
    }
}
