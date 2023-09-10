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

            var lineupsBag = new ConcurrentBag<LineupModel>();

            long iterationCount = 0;
            long validLineupCount = 0;

            var cancellationTokenSource = new CancellationTokenSource();

            var outputTask = Task.Factory.StartNew(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(10000);

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

            return eligiblePlayers.ToList();
        }
    }
}
