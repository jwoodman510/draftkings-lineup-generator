using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IIncrementalLineupLogger
    {
        Task StartAsync<T>(string logger, List<T> players, CancellationToken cancellationToken) where T : DraftableModel;

        Task StopAsync(CancellationToken cancellationToken);

        void IncrementIterations();

        void IncrementValidLineups();

        (long iterationCount, long validLineupCount) GetProgress();
    }
}
