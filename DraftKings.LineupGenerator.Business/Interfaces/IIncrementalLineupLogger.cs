using DraftKings.LineupGenerator.Models.Draftables;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IIncrementalLineupLogger
    {
        Task StartAsync(string logger, List<DraftableModel> players, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        void IncrementIterations();

        void IncrementValidLineups();
    }
}
