using DraftKings.LineupGenerator.Models.Contests;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public class ContestsClient : ClientBase, IContestsClient
    {
        public ContestsClient(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
            : base(memoryCache, httpClientFactory) { }

        public Task<ContestModel> GetAsync(int contestId) =>
            GetAsync<ContestModel>($"https://api.draftkings.com/contests/v1/contests/{contestId}?format=json");
    }
}
