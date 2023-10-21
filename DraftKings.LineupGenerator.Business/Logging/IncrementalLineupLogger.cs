using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Draftables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Logging
{
    public class IncrementalLineupLogger : IIncrementalLineupLogger
    {
        private long _iterationCount = 0;
        private long _validLineupCount = 0;
        private Task _consoleLoggerTask;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly ILogger<IncrementalLineupLogger> _logger;

        public IncrementalLineupLogger(ILogger<IncrementalLineupLogger> logger)
        {
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(string logger, List<DraftableModel> players, CancellationToken cancellationToken)
        {
            _logger?.LogInformationGreen($"Running Generator: {logger} for {players.Count} players");

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
            _cancellationTokenSource?.Cancel();

            if (_consoleLoggerTask != null)
            {
                await _consoleLoggerTask;
            }
        }

        public void IncrementIterations() => Interlocked.Add(ref _iterationCount, 1);

        public void IncrementValidLineups() => Interlocked.Add(ref _validLineupCount, 1);

        private Task LogIterationAsync(string logger, CancellationToken _)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            _logger?.LogInformation($"[{{0}}] | {{1}} | Iterations: {{2}} | Valid Lineups: {{3}}", now, logger, _iterationCount, _validLineupCount);

            return Task.CompletedTask;
        }
    }
}
