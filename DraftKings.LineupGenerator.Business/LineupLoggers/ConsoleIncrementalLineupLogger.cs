using DraftKings.LineupGenerator.Business.Interfaces;
using System;
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

        public ConsoleIncrementalLineupLogger()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(string logger, CancellationToken cancellationToken)
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

        private Task LogIterationAsync(string logger, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now:T}] | {logger} | Iterations: {_iterationCount:n0} | Valid Lineups: {_validLineupCount:n0}");

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
