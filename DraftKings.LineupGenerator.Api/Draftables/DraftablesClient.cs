using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Models.Draftables;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public class DraftablesClient : ClientBase, IDraftablesClient
    {
        public DraftablesClient(
            ICacheService cacheService,
            IHttpClientFactory httpClientFactory)
            : base(cacheService, httpClientFactory) { }

        public Task<DraftablesModel> GetAsync(int contestId) =>
            GetAsync<DraftablesModel>($"https://api.draftkings.com/draftgroups/v1/draftgroups/{contestId}/draftables?format=json");
    }
}
