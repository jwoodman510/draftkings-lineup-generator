using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using DraftKings.LineupGenerator.Business.Filters;
using System.Linq;
using System;
using DraftKings.LineupGenerator.Business.LinupBags;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Business.LineupGenerators
{
    public abstract class BaseLineupGenerator<TDraftableModel> : ILineupGenerator where TDraftableModel : DraftableModel
    {
        protected abstract string Description { get; }

        protected readonly ILineupService LineupService;
        protected readonly IIncrementalLineupLogger IncrementalLogger;
        protected readonly ConcurrentDictionary<string, BaseLineupsBag> LineupsBags;

        protected BaseLineupGenerator(
            ILineupService lineupService,
            IIncrementalLineupLogger incrementalLogger,
            params BaseLineupsBag[] lineupsBags)
        {
            LineupService = lineupService;
            IncrementalLogger = incrementalLogger;
            LineupsBags = new ConcurrentDictionary<string, BaseLineupsBag>(lineupsBags.Select(x => new KeyValuePair<string, BaseLineupsBag>(x.Description, x)));
        }

        public abstract bool CanGenerate(ContestModel contest, RulesModel rules);

        protected abstract List<TDraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables);

        protected virtual bool IsValidLineup(LineupRequestModel request, RulesModel rules, DraftablesModel draftables, LineupModel lineup)
        {
            if (!lineup.MeetsSalaryCap(rules))
            {
                return false;
            }

            if (!lineup.IncludesPlayerRequests(request.PlayerRequests))
            {
                return false;
            }

            return true;
        }

        public IEnumerable<LineupsModel> GetCurrentLineups(LineupRequestModel request) => LineupsBags.Select(lineupsBag => new LineupsModel
        {
            Description = lineupsBag.Key,
            Lineups = lineupsBag.Value.GetBestLineups(request.LineupCount).ToList()
        });

        public async Task<IEnumerable<LineupsModel>> GenerateAsync(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, CancellationToken cancellationToken)
        {
            if (LineupsBags.Count == 0 || draftables.Draftables.All(x => x.Salary == default))
            {
                return LineupsBags.Select(x => new LineupsModel
                {
                    Description = x.Key
                });
            }

            var eligiblePlayers = GetEligiblePlayers(request, rules, draftables);

            var potentialLineups = LineupService.GetPotentialLineups(request, rules, draftables, eligiblePlayers);

            await IncrementalLogger.StartAsync(Description, eligiblePlayers, cancellationToken);

            try
            {
                var opts = new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = 4
                };

                Parallel.ForEach(potentialLineups, opts, potentialLineup =>
                {
                    IncrementalLogger.IncrementIterations();

                    var lineup = new LineupModel(potentialLineup.Select(player => player.ToDisplayModel(rules, draftables)));

                    if (!IsValidLineup(request, rules, draftables, lineup))
                    {
                        return;
                    }

                    IncrementalLogger.IncrementValidLineups();

                    foreach (var lineupsBag in LineupsBags)
                    {
                        lineupsBag.Value.UpdateLineups(contest, lineup, request.LineupCount);
                    }
                });
            }
            catch (OperationCanceledException) { }
            finally
            {
                await IncrementalLogger.StopAsync(cancellationToken);
            }

            return LineupsBags.Select(x => new LineupsModel
            {
                Description = x.Key,
                Lineups = x.Value.GetBestLineups(request.LineupCount).ToList()
            });
        }
    }
}
