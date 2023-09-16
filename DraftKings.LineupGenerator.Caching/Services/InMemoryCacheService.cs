using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Caching.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory, CancellationToken cancellationToken)
            where T : class
        {
            if (_cache.TryGetValue<T>(key, out var cachedValue))
            {
                return cachedValue;
            }

            var value = await valueFactory();

            var entry = _cache.CreateEntry(value);

            entry.SetAbsoluteExpiration(expiration);

            return value;
        }
    }
}
