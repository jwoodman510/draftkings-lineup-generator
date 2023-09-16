using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Models.Rules;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Rules
{
    public class RulesClient : ClientBase, IRulesClient
    {
        public RulesClient(
            ICacheService cacheService,
            IHttpClientFactory httpClientFactory)
            : base(cacheService, httpClientFactory) { }

        public Task<RulesModel> GetAsync(int contestId, CancellationToken cancellationToken) =>
            GetAsync<RulesModel>($"https://api.draftkings.com/lineups/v1/gametypes/{contestId}/rules?format=json", cancellationToken);
    }
}
