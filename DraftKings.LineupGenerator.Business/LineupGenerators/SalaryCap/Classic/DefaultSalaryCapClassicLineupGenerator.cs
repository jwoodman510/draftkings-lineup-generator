using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Concurrent;
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

            return
                rules.GameTypeName == GameTypes.NflClassic ||
                rules.GameTypeName == GameTypes.XflClassic ||
                rules.GameTypeName == GameTypes.MaddenClassic;
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

            var eligiblePlayers = GetEligiblePlayers(request, rules, draftables);

            var potentialLineups = _classicLineupService.GetPotentialLineups(rules, draftables, eligiblePlayers);

            var lineupsBag = new ConcurrentBag<LineupModel>();

            potentialLineups.AsParallel().ForAll(potentialLineup =>
            {
                var lineup = new LineupModel
                {
                    Draftables = potentialLineup
                        .Select(player => new DraftableDisplayModel(
                            player.DisplayName,
                            player.GetFppg(draftables.DraftStats),
                            player.Salary,
                            player.GetRosterPosition(rules)))
                        .ToList()
                };

                if (lineup.Salary >= rules.SalaryCap.MaxValue || lineup.Salary <= rules.SalaryCap.MinValue)
                {
                    return;
                }

                if (lineupsBag.IsEmpty)
                {
                    lineupsBag.Add(lineup);

                    return;
                }

                if (!lineupsBag.TryPeek(out var existing))
                {
                    return;
                }

                if (existing.Fppg > lineup.Fppg)
                {
                    return;
                }

                if (existing.Fppg < lineup.Fppg)
                {
                    lineupsBag.Clear();
                }

                lineupsBag.Add(lineup);
            });

            result.Lineups.AddRange(lineupsBag);

            return result;
        }

        private static List<DraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables)
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

            if (!request.IncludeBaseSalary)
            {
                eligiblePlayers.ExcludeBaseSalary();
            }

            var dstRosterSlot = rules.LineupTemplate.Single(x => x.RosterSlot.Name == RosterSlots.Dst).RosterSlot;

            var nonDstPlayers = eligiblePlayers
                .Where(x => x.RosterSlotId != dstRosterSlot.Id)
                .MinimumFppg(draftables.DraftStats, request.MinFppg);

            var dstPlayers = eligiblePlayers.Where(x => x.RosterSlotId == dstRosterSlot.Id);

            return nonDstPlayers.Concat(dstPlayers).ToList();
        }
    }
}
