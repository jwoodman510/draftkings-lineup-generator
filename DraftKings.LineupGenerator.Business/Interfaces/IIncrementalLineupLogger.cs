using DraftKings.LineupGenerator.Business.LinupBags;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IIncrementalLineupLogger
    {
        Task StartAsync(string format, BaseLineupsBag lineupsBag, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        void IncrementIterations();

        void IncrementValidLineups();
    }
}
