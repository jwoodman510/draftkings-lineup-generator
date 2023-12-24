using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Models.Contests;
using System.Collections.Generic;
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

        public Task<ContestModel> GetAsync(int contestId, CancellationToken cancellationToken) =>
            GetAsync<ContestModel>($"https://api.draftkings.com/contests/v1/contests/{contestId}?format=json", cancellationToken);

        public Task<IEnumerable<ContestSearchModel>> SearchAsync(string sport, CancellationToken cancellation) =>
            GetAsync<IEnumerable<ContestSearchModel>>($"https://www.draftkings.com/lobby/getcontests?sport={sport}", cancellation);
    }
}
