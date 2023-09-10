using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap classic contests based on FPPG.
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapClassicLineupGenerator : ILineupGenerator
    {
        private readonly IClassicLineupService _classicLineupService;

        public DefaultSalaryCapClassicLineupGenerator(IClassicLineupService classicLineupService)
        {
            _classicLineupService = classicLineupService;
        }

        public bool CanGenerate(ContestModel contest, RulesModel rules)
        {
            if (contest.ContestDetail.Sport != Sports.Nfl)
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

            long iterationCount = 0;
            long validLineupCount = 0;

            var cancellationTokenSource = new CancellationTokenSource();

            var outputTask = Task.Factory.StartNew(async () =>
            {
                LineupModel lineupModel = null;

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(10000);

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    Console.WriteLine($"[{DateTime.Now:T}]\tIterations: {iterationCount:n0} | Valid Lineups: {validLineupCount:n0}");

                    if (lineupsBag.TryPeek(out var lineup) && lineup != lineupModel)
                    {
                        lineupModel = lineup;
                        Console.WriteLine("Best Current Lineup:");
                        Console.WriteLine(JsonConvert.SerializeObject(lineup, Formatting.Indented));
                    }
                }
            }, TaskCreationOptions.LongRunning);

            potentialLineups.AsParallel().ForAll(potentialLineup =>
            {
                Interlocked.Add(ref iterationCount, 1);

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

                Interlocked.Add(ref validLineupCount, 1);

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

            cancellationTokenSource.Cancel();

            await outputTask.ConfigureAwait(false);

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
                eligiblePlayers.ExcludeBaseSalaryByPosition();
            }

            var dstRosterSlot = rules.LineupTemplate.Single(x => x.RosterSlot.Name == RosterSlots.Dst).RosterSlot;

            var nonDstPlayers = eligiblePlayers
                .Where(x => x.RosterSlotId != dstRosterSlot.Id)
                .MinimumFppg(draftables.DraftStats, request.MinFppg);

            var dstPlayers = eligiblePlayers.Where(x => x.RosterSlotId == dstRosterSlot.Id);

            // TODO: Filter out backup QBs

            return nonDstPlayers.Concat(dstPlayers).ToList();
        }
    }
}
