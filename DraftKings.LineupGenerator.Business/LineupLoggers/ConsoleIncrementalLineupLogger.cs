using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LinupBags;
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

        public Task StartAsync(string format, BaseLineupsBag lineupsBag, CancellationToken cancellationToken)
        {
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

                    await LogIterationAsync(_cancellationTokenSource.Token);

                    var bestLineup = lineupsBag.GetBestLineup();

                    if (bestLineup != null)
                    {
                        await LogLineupAsync(format, "Best Current Lineup:", bestLineup, _cancellationTokenSource.Token);
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

            WriteLine(description, ConsoleColor.Yellow);

            var lineupOutput = await outputFormatter?.FormatLineupAsync(new[] { lineup }, cancellationToken);

            WriteLine(lineupOutput, ConsoleColor.Magenta);
        }

        private Task LogIterationAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now:T}]\tIterations: {_iterationCount:n0} | Valid Lineups: {_validLineupCount:n0}");

            return Task.CompletedTask;
        }

        private static void WriteLine(string message, ConsoleColor foregroundColor)
        {
            var defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(message);

            Console.ForegroundColor = defaultColor;
        }
    }
}
