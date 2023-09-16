using DraftKings.LineupGenerator.Caching;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api
{
    public class ClientBase
    {
        private readonly ICacheService _cache;
        protected readonly IHttpClientFactory HttpClientFactory;

        public ClientBase(
            ICacheService cache,
            IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            HttpClientFactory = httpClientFactory;
        }

        protected async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken) where T : class
        {
            var cacheKey = url;

            return await _cache.GetOrCreateAsync<T>(url, TimeSpan.FromMinutes(5), async () =>
            {
                var response = await HttpClientFactory.CreateClient().GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrEmpty(responseJson))
                {
                    return default;
                }

                var result = JsonConvert.DeserializeObject<T>(responseJson);

                return result;
            }, cancellationToken);
        }
    }
}
