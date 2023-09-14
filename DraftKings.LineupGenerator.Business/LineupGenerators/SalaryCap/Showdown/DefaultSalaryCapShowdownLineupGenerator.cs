using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap showdown contests based on FPPG.
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapShowdownLineupGenerator : ILineupGenerator
    {
        private readonly IShowdownLineupService _showdownLineupService;

        public DefaultSalaryCapShowdownLineupGenerator(IShowdownLineupService showdownLineupService)
        {
            _showdownLineupService = showdownLineupService;
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
                rules.GameTypeName == GameTypes.Showdown ||
                rules.GameTypeName == GameTypes.NflShowdown ||
                rules.GameTypeName == GameTypes.XflShowdown ||
                rules.GameTypeName == GameTypes.MaddenShowdown;
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

            var potentialLineups = _showdownLineupService.GetPotentialLineups(rules, draftables, eligiblePlayers);

            var lineupsBag = new ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>>();

            long iterationCount = 0;
            long validLineupCount = 0;

            var cancellationTokenSource = new CancellationTokenSource();

            var outputTask = Task.Factory.StartNew(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(10000);

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    Console.WriteLine($"[{DateTime.Now:T}]\tIterations: {iterationCount:n0} | Valid Lineups: {validLineupCount:n0}");
                }
            }, TaskCreationOptions.LongRunning);

            potentialLineups.AsParallel().ForAll(potentialLineup =>
            {
                Interlocked.Add(ref iterationCount, 1);

                var lineup = new LineupModel
                {
                    Draftables = potentialLineup
                        .Select((player, index) => new DraftableDisplayModel(
                            player.DisplayName,
                            player.GetFppg(draftables.DraftStats),
                            player.Salary,
                            player.GetRosterPosition(rules),
                            GetProjectedSalary(draftables, player, rules)))
                        .ToList()
                };

                if (lineup.Salary >= rules.SalaryCap.MaxValue || lineup.Salary <= rules.SalaryCap.MinValue)
                {
                    return;
                }

                Interlocked.Add(ref validLineupCount, 1);

                var minKey = lineupsBag.Keys.Count == 0 ? 0 : lineupsBag.Keys.Min();

                if (lineup.ProjectedFppg < minKey)
                {
                    return;
                }

                var lineups = lineupsBag.GetOrAdd(lineup.ProjectedFppg, _ => new ConcurrentBag<LineupModel>());

                if (lineups.Count < 5)
                {
                    lineups.Add(lineup);
                }

                if (lineupsBag.Keys.Count > 5) 
                {
                    lineupsBag.TryRemove(minKey, out _);
                }
            });

            result.Lineups.AddRange(lineupsBag.Values.SelectMany(x => x).OrderBy(x => x.ProjectedFppg).Take(5));

            cancellationTokenSource.Cancel();

            return result;
        }

        private static decimal GetProjectedSalary(DraftablesModel draftables, DraftableModel draftable, RulesModel rules)
        {
            var captainSlot = rules.LineupTemplate.First(x => x.RosterSlot.Name == RosterSlots.Captain);
            var projectedFppg = draftable.GetFppg(draftables.DraftStats);

            return draftable.RosterSlotId == captainSlot.RosterSlot.Id
                ? projectedFppg * 1.5m
                : projectedFppg;
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
