using DraftKings.LineupGenerator.Models.Rules;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Rules
{
    public class RulesClient : ClientBase, IRulesClient
    {
        public RulesClient(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
            : base(memoryCache, httpClientFactory) { }

        public Task<RulesModel> GetAsync(int contestId) =>
            GetAsync<RulesModel>($"https://api.draftkings.com/lineups/v1/gametypes/{contestId}/rules?format=json");
    }
}
