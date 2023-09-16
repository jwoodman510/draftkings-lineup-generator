using DraftKings.LineupGenerator.Models.Lineups;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IIncrementalLineupLogger
    {
        void IncrementIterations();

        void IncrementValidLineups();

        Task LogIterationAsync(CancellationToken cancellationToken);

        Task LogLineupAsync(string format, string description, LineupModel lineup, CancellationToken cancellationToken);
    }
}
