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
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.LinupBags;

namespace DraftKings.LineupGenerator.Business.LineupGenerators
{
    public abstract class BaseLineupGenerator : ILineupGenerator
    {
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

        protected virtual void ModifyLineup(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, LineupModel lineup)
        {

        }

        public LineupsModel GetCurrentLineups(LineupRequestModel request) => new LineupsModel
        {
            Description = LineupsBag.Description,
            Lineups = LineupsBag.GetBestLineups(request.LineupCount).ToList()
        };

        public async Task<LineupsModel> GenerateAsync(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, CancellationToken cancellationToken)
        {
            var result = new LineupsModel
            {
                Description = LineupsBag.Description
            };

            if (draftables.Draftables.All(x => x.Salary == default))
            {
                return result;
            }

            var eligiblePlayers = GetEligiblePlayers(request, rules, draftables);

            var potentialLineups = LineupService.GetPotentialLineups(request, rules, draftables, eligiblePlayers);

            await IncrementalLogger.StartAsync(LineupsBag.Description, eligiblePlayers, cancellationToken);

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

                    var lineup = new LineupModel
                    {
                        Draftables = potentialLineup
                            .Select(player => player.ToDisplayModel(rules, draftables))
                            .ToList()
                    };

                    if (!IsValidLineup(request, rules, draftables, lineup))
                    {
                        return;
                    }

                    ModifyLineup(request, contest, rules, draftables, lineup);

                    IncrementalLogger.IncrementValidLineups();

                    LineupsBag.UpdateLineups(contest, lineup, request.LineupCount);
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
