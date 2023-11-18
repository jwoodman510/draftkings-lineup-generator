using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Models.Contests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public class ContestsClient : ClientBase, IContestsClient
    {
        public ContestsClient(
            ICacheService cacheService,
            IHttpClientFactory httpClientFactory)
            : base(cacheService, httpClientFactory) { }

        public async Task<ContestModel> GetAsync(int contestId, CancellationToken cancellationToken)
        {
            var contest = await GetAsync<ContestModel>($"https://api.draftkings.com/contests/v1/contests/{contestId}?format=json", cancellationToken);

            if (contest != null)
            {
                contest.Id = contestId;
            }

            return contest;
        }
    }
}
