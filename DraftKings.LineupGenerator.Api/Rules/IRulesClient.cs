using DraftKings.LineupGenerator.Models.Rules;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Rules
{
    public interface IRulesClient
    {
        Task<RulesModel> GetAsync(int contestId, CancellationToken cancellationToken);
    }
}
