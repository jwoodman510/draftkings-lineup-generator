using DraftKings.LineupGenerator.Models.Contests;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public interface IContestsClient
    {
        Task<ContestModel> GetAsync(int contestId, CancellationToken cancellationToken);

        Task<ContestsSearchModel> SearchAsync(string sport, CancellationToken cancellation);
    }
}
