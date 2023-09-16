using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.LineupLoggers
{
    public class ConsoleIncrementalLineupLogger : IIncrementalLineupLogger
    {
        private long _iterationCount = 0;
        private long _validLineupCount = 0;
        private Task _consoleLoggerTask;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IEnumerable<IOutputFormatter> _outputFormatters;

        public ConsoleIncrementalLineupLogger(IEnumerable<IOutputFormatter> outputFormatters)
        {
            _outputFormatters = outputFormatters;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(string format, LineupsBag lineupsBag, CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _consoleLoggerTask = Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    LineupModel bestLineup = null;

                    await Task.Delay(TimeSpan.FromSeconds(10));

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    await LogIterationAsync(_cancellationTokenSource.Token);

                    if (lineupsBag?.Count > 0)
                    {
                        var maxKey = lineupsBag.Keys.Max();
                        var lineup = lineupsBag[maxKey].OrderByDescending(x => x.ProjectedFppg).First();

                        if (lineup != bestLineup)
                        {
                            bestLineup = lineup;

                            await LogLineupAsync(format, "Best Current Lineup:", bestLineup, _cancellationTokenSource.Token);
                        }
                    }
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

        private async Task LogLineupAsync(string format, string description, LineupModel lineup, CancellationToken cancellationToken)
        {
            var outputFormatter = _outputFormatters.FirstOrDefault(x => x.Type.Equals(format))
                ?? _outputFormatters.FirstOrDefault();

            Console.WriteLine(description);

            var lineupOutput = await outputFormatter?.FormatAsync(new[] { lineup }, cancellationToken);

            Console.WriteLine(lineupOutput);
        }

        private Task LogIterationAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now:T}]\tIterations: {_iterationCount:n0} | Valid Lineups: {_validLineupCount:n0}");

            return Task.CompletedTask;
        }
    }
}
