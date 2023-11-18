using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Services
{
    public class LineupGeneratorService : ILineupGeneratorService
    {
        private readonly IDraftKingsClient _draftKingsClient;
        private readonly ILogger<LineupGeneratorService> _logger;
        private readonly IEnumerable<IOutputFormatter> _formatters;
        private readonly IEnumerable<ILineupGenerator> _lineupGenerators;

        public LineupGeneratorService(
            IDraftKingsClient draftKingsClient,
            ILogger<LineupGeneratorService> logger,
            IEnumerable<IOutputFormatter> formatters,
            IEnumerable<ILineupGenerator> lineupGenerators)
        {
            _logger = logger;
            _formatters = formatters;
            _draftKingsClient = draftKingsClient;
            _lineupGenerators = lineupGenerators;
        }

        public async Task<List<LineupsModel>> GetAsync(LineupRequestModel request, CancellationToken cancellationToken)
        {
            IDisposable logScope = null;

            if (request.CorrelationId == null)
            {
                logScope = _logger.BeginScope(new Dictionary<string, object>
                {
                    { "correlationId", Guid.NewGuid() }
                });
            }

            var formatter = _formatters.FirstOrDefault(x => x.Type.Equals(request.OutputFormat, StringComparison.OrdinalIgnoreCase))
                ?? _formatters.First();

            _logger.LogInformationGreen("Generating Lineups for Configuration:");

            _logger.LogInformation(await formatter.FormatAsync(request, cancellationToken));

            var contest = await _draftKingsClient.Contests.GetAsync(request.ContestId, cancellationToken);

            if (contest == null)
            {
                return new List<LineupsModel>();
            }

            _logger.LogInformation("Contest Found: {ContestName}", contest.ContestDetail.Name);

            var rules = await _draftKingsClient.Rules.GetAsync(contest.ContestDetail.GameTypeId, cancellationToken);
            var draftables = await _draftKingsClient.Draftables.GetAsync(contest.ContestDetail.DraftGroupId, cancellationToken);

            if (rules == null || draftables == null)
            {
                return new List<LineupsModel>();
            }

            var generators = _lineupGenerators.Where(x => x.CanGenerate(contest, rules)).AsParallel();

            var tasks = generators.Select(x => x.GenerateAsync(request, contest, rules, draftables, cancellationToken));

            var lineups = await Task.WhenAll(tasks);

            logScope?.Dispose();

            return lineups.SelectMany(x => x).ToList();
        }

        public async Task LogProgressAsync(LineupRequestModel request, CancellationToken cancellationToken)
        {
            var formatter = _formatters.FirstOrDefault(x => x.Type.Equals(request.OutputFormat, StringComparison.OrdinalIgnoreCase))
                ?? _formatters.First();

            var lineupsModels = _lineupGenerators
                .Select(x => x.GetCurrentLineups(request))
                .SelectMany(x => x)
                .Where(x => x?.Lineups?.Count > 0)
                .ToList();

            if (lineupsModels.Count > 0)
            {
                foreach (var lineupsModel in lineupsModels)
                {
                    _logger.LogInformationGreen($"Generator Results: {lineupsModel.Description}");
                    _logger.LogInformation(await formatter.FormatLineupsAsync(lineupsModel.Lineups, cancellationToken));
                }
            }
            else
            {
                _logger.LogWarning("No Lineups Found.");
            }
        }
    }
}
