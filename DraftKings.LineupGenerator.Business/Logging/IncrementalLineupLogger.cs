using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.Metrics;
using DraftKings.LineupGenerator.Models.Draftables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DraftKings.LineupGenerator.Business.Logging
{
    public class IncrementalLineupLogger : IIncrementalLineupLogger
    {
        private string _action;
        private long _iterationCount = 0;
        private Stopwatch _actionStopwatch;
        private long _validLineupCount = 0;
        private Task _consoleLoggerTask;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IMetricsService _metricsService;
        private readonly ILogger<IncrementalLineupLogger> _logger;

        public IncrementalLineupLogger(
            IMetricsService metricsService,
            ILogger<IncrementalLineupLogger> logger)
        {
            _logger = logger;
            _metricsService = metricsService;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(string logger, List<DraftableModel> players, CancellationToken cancellationToken)
        {
            _action = $"Generator: {logger}";
            _actionStopwatch = Stopwatch.StartNew();

            _metricsService.StartAction(_action);

            _logger?.LogInformation("Running Generator: {Generator} for {EligiblePlayerCount} players.", logger, players.Count);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _consoleLoggerTask = Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    await LogIterationAsync(logger, _cancellationTokenSource.Token);
                }
            }, TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _metricsService.EndAction(_action);
            _cancellationTokenSource?.Cancel();

            if (_consoleLoggerTask != null)
            {
                await _consoleLoggerTask;
            }
        }

        public void IncrementIterations()
        {
            const int metricsInterval = 1000000;

            Interlocked.Add(ref _iterationCount, 1);

            var iterations = Interlocked.Read(ref _iterationCount);

            if (iterations >= metricsInterval && iterations % metricsInterval == 0)
            {
                _actionStopwatch.Stop();

                _metricsService.LogAction($"{_action}: {metricsInterval}", _actionStopwatch.Elapsed);

                _actionStopwatch.Restart();
            }
        }

        public void IncrementValidLineups() => Interlocked.Add(ref _validLineupCount, 1);

        private Task LogIterationAsync(string logger, CancellationToken _)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            _logger?.LogInformation($"[{{Time}}] | {{Generator}} | Iterations: {{IterationCount}} | Valid Lineups: {{ValidLineupCount}}", now, logger, _iterationCount, _validLineupCount);

            return Task.CompletedTask;
        }
    }
}
