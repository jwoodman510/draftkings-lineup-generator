using DraftKings.LineupGenerator.Models.Draftables;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public class DraftablesClient : ClientBase, IDraftablesClient
    {
        public DraftablesClient(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
            : base(memoryCache, httpClientFactory) { }

        public Task<DraftablesModel> GetAsync(int contestId) =>
            GetAsync<DraftablesModel>($"https://api.draftkings.com/draftgroups/v1/draftgroups/{contestId}/draftables?format=json");
    }
}
