using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using DraftKings.LineupGenerator.Business.Filters;
using DraftKings.LineupGenerator.Business.LineupBags;
using System.Linq;
using System;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.LinupBags;

namespace DraftKings.LineupGenerator.Business.LineupGenerators
{
    public abstract class BaseLineupGenerator : ILineupGenerator
    {
        protected abstract string Description { get; }

        protected readonly BaseLineupsBag LineupsBag;
        protected readonly ILineupService LineupService;
        protected readonly IIncrementalLineupLogger IncrementalLogger;

        protected BaseLineupGenerator(
            BaseLineupsBag lineupsBag,
            ILineupService lineupService,
            IIncrementalLineupLogger incrementalLogger)
        {
            LineupsBag = lineupsBag;
            LineupService = lineupService;
            IncrementalLogger = incrementalLogger;
        }

        public abstract bool CanGenerate(ContestModel contest, RulesModel rules);

        protected abstract List<DraftableModel> GetEligiblePlayers(LineupRequestModel request, RulesModel rules, DraftablesModel draftables);

        public async Task<LineupsModel> GenerateAsync(LineupRequestModel request, RulesModel rules, DraftablesModel draftables, CancellationToken cancellationToken)
        {
            var result = new LineupsModel
            {
                Description = Description
            };

            if (draftables.Draftables.All(x => x.Salary == default))
            {
                return result;
            }

            var eligiblePlayers = GetEligiblePlayers(request, rules, draftables);

            var potentialLineups = LineupService.GetPotentialLineups(rules, draftables, eligiblePlayers);

            await IncrementalLogger.StartAsync(request.OutputFormat, LineupsBag, cancellationToken);

            try
            {
                var opts = new ParallelOptions
                {
                    CancellationToken = cancellationToken
                };

                Parallel.ForEach(potentialLineups, opts, potentialLineup =>
                {
                    IncrementalLogger.IncrementIterations();

                    var lineup = new LineupModel
                    {
                        Draftables = potentialLineup
                            .Select(player => player.ToDisplayModel(rules, draftables))
                            .ToList()
                    };

                    if (!lineup.MeetsSalaryCap(rules))
                    {
                        return;
                    }

                    if (!lineup.IncludesPlayerRequests(request.PlayerRequests))
                    {
                        return;
                    }

                    IncrementalLogger.IncrementValidLineups();

                    LineupsBag.UpdateLineups(lineup, request.LineupCount);
                });
            }
            catch (OperationCanceledException) { }
            finally
            {
                await IncrementalLogger.StopAsync(cancellationToken);
            }

            result.Lineups.AddRange(LineupsBag.GetBestLineups(request.LineupCount));

            return result;
        }
    }
}
