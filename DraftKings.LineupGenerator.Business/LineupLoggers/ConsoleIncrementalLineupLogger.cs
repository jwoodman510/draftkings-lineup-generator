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

        private readonly IEnumerable<IOutputFormatter> _outputFormatters;

        public ConsoleIncrementalLineupLogger(IEnumerable<IOutputFormatter> outputFormatters)
        {
            _outputFormatters = outputFormatters;
        }

        public void IncrementIterations()
        {
            Interlocked.Add(ref _iterationCount, 1);
        }

        public void IncrementValidLineups()
        {
            Interlocked.Add(ref _validLineupCount, 1);
        }

        public async Task LogLineupAsync(string format, string description, LineupModel lineup, CancellationToken cancellationToken)
        {
            var outputFormatter = _outputFormatters.FirstOrDefault(x => x.Type.Equals(format))
                ?? _outputFormatters.FirstOrDefault();

            Console.WriteLine(description);

            var lineupOutput = await outputFormatter?.FormatAsync(new[] { lineup }, cancellationToken);

            Console.WriteLine(lineupOutput);
        }

        public Task LogIterationAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now:T}]\tIterations: {_iterationCount:n0} | Valid Lineups: {_validLineupCount:n0}");

            return Task.CompletedTask;
        }
    }
}
