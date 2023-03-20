using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Models.Contests;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public class ContestsClient : ClientBase, IContestsClient
    {
        public ContestsClient(
            ICacheService cacheService,
            IHttpClientFactory httpClientFactory)
            : base(cacheService, httpClientFactory) { }

        public Task<ContestModel> GetAsync(int contestId) =>
            GetAsync<ContestModel>($"https://api.draftkings.com/contests/v1/contests/{contestId}?format=json");
    }
}
