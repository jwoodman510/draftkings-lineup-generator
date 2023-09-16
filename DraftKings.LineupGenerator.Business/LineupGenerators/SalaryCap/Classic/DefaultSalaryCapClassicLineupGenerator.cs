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
    /// The default lineup generator for salary cap classic contests based on FPPG.
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapClassicLineupGenerator : ILineupGenerator
    {
        private readonly IClassicLineupService _classicLineupService;
        private readonly IIncrementalLineupLogger _incrementalLogger;

        public DefaultSalaryCapClassicLineupGenerator(
            IClassicLineupService classicLineupService,
            IIncrementalLineupLogger incrementalLogger)
        {
            _classicLineupService = classicLineupService;
            _incrementalLogger = incrementalLogger;
        }

        public bool CanGenerate(ContestModel contest, RulesModel rules)
        {
            if (contest.ContestDetail.Sport != Sports.Nfl &&
                contest.ContestDetail.Sport != Sports.Xfl)
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

            var lineupsBag = new ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>>();

            var cancellationTokenSource = new CancellationTokenSource();

            var outputTask = Task.Factory.StartNew(async () =>
            {
                LineupModel bestLineup = null;

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    await _incrementalLogger.LogIterationAsync(cancellationTokenSource.Token);

                    if (lineupsBag.Count > 0)
                    {
                        var maxKey = lineupsBag.Keys.Max();
                        var lineup = lineupsBag[maxKey].OrderByDescending(x => x.ProjectedFppg).First();

                        if (lineup != bestLineup)
                        {
                            bestLineup = lineup;

                            await _incrementalLogger.LogLineupAsync(request.OutputFormat, "Best Current Lineup:", bestLineup, cancellationTokenSource.Token);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            potentialLineups.AsParallel().ForAll(potentialLineup =>
            {
                _incrementalLogger.IncrementIterations();

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

                _incrementalLogger.IncrementValidLineups();

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

            await outputTask;

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
                eligiblePlayers = eligiblePlayers.ExcludeQuestionable();
            }

            if (!request.IncludeBaseSalary)
            {
                eligiblePlayers = eligiblePlayers.ExcludeBaseSalaryByPosition();
            }

            var dstRosterSlot = rules.LineupTemplate.Single(x => x.RosterSlot.Name == RosterSlots.Dst).RosterSlot;

            var remainingPlayers = eligiblePlayers
                .Where(x => x.RosterSlotId != dstRosterSlot.Id)
                .Where(x => x.Position != RosterSlots.Quarterback)
                .MinimumFppg(draftables.DraftStats, request.MinFppg);

            var dstPlayers = eligiblePlayers.Where(x => x.RosterSlotId == dstRosterSlot.Id);
            var quarterbacks = eligiblePlayers.Where(x => x.Position == RosterSlots.Quarterback);

            var highestSalaryQuarterbacksByTeam = quarterbacks.GroupBy(x => x.TeamId).Select(x => x.OrderByDescending(y => y.Salary).First());

            return remainingPlayers.Concat(dstPlayers).Concat(highestSalaryQuarterbacksByTeam).ToList();
        }
    }
}
