using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api
{
    public class ClientBase
    {
        private readonly IMemoryCache _memoryCache;

        protected readonly IHttpClientFactory HttpClientFactory;

        public ClientBase(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            HttpClientFactory = httpClientFactory;
        }

        protected async Task<T> GetAsync<T>(string url) where T : class
        {
            var cacheKey = url;

            if (_memoryCache.TryGetValue<T>(url, out var cachedValue))
            {
                return cachedValue;
            }

            var response = await HttpClientFactory.CreateClient().GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseJson))
            {
                return default;
            }

            var result = JsonConvert.DeserializeObject<T>(responseJson);

            _memoryCache.CreateEntry(cacheKey);

            return result;
        }
    }
}
