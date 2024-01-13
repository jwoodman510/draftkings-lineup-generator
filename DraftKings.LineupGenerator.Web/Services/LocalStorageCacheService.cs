using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Caching;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public class LocalStorageCacheService : ICacheService
    {
        private const string CacheKeyPrefix = "_cache_";
        private const string CacheKeysPath = $"{CacheKeyPrefix}:keys";
        private const string CacheExpirationsPath = $"{CacheKeyPrefix}:expirations";

        private static SemaphoreSlim _keySemaphore = new SemaphoreSlim(1);

        private readonly ILocalStorageService _localStorageService;

        public LocalStorageCacheService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            await _keySemaphore.WaitAsync(cancellationToken);

            try
            {
                var keys = await _localStorageService.GetItemAsync<HashSet<string>>(CacheKeyPrefix, cancellationToken);

                foreach (var key in keys)
                {
                    await _localStorageService.RemoveItemAsync(key, cancellationToken);
                }

                await _localStorageService.RemoveItemAsync(CacheKeysPath, cancellationToken);
                await _localStorageService.RemoveItemAsync(CacheExpirationsPath, cancellationToken);
            }
            finally
            {
                _keySemaphore.Release();
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory, CancellationToken cancellationToken) where T : class
        {
            await _keySemaphore.WaitAsync(cancellationToken);

            try
            {
                var itemKey = await GetKeyMetadataAsync(key, CacheKeysPath, () => $"{Guid.NewGuid()}.json", cancellationToken);
                var absoluteExpiration = await GetKeyMetadataAsync(key, CacheExpirationsPath, () => DateTime.UtcNow.Add(expiration).ToString(), cancellationToken);

                var cacheKey = $"{CacheKeyPrefix}:{itemKey}";
                var cacheHit = await _localStorageService.ContainKeyAsync(cacheKey);

                if (cacheHit && DateTime.UtcNow <= DateTime.Parse(absoluteExpiration))
                {
                    return await _localStorageService.GetItemAsync<T>(cacheKey, cancellationToken);
                }

                if (cacheHit && DateTime.UtcNow > DateTime.Parse(absoluteExpiration))
                {
                    absoluteExpiration = DateTime.UtcNow.Add(expiration).ToString();
                }

                var value = await valueFactory();

                await _localStorageService.SetItemAsync(cacheKey, value, cancellationToken);

                await WriteKeyMetadataAsync(key, CacheExpirationsPath, absoluteExpiration.ToString(), cancellationToken);

                return value;
            }
            finally
            {
                _keySemaphore.Release();
            }
        }

        private async Task<string> GetKeyMetadataAsync(string key, string metadataKey, Func<string> metadataValueFactory, CancellationToken cancellationToken)
        {
            var metadata = await _localStorageService.ContainKeyAsync(metadataKey)
                ? await _localStorageService.GetItemAsync<Dictionary<string, string>>(metadataKey, cancellationToken)
                : new Dictionary<string, string>();

            if (!metadata.ContainsKey(key))
            {
                metadata[key] = metadataValueFactory();
            }

            await _localStorageService.SetItemAsync(key, metadata, cancellationToken);

            return metadata[key];
        }

        private async Task WriteKeyMetadataAsync(string key, string metadataKey, string value, CancellationToken cancellationToken)
        {
            var metadata = await _localStorageService.ContainKeyAsync(metadataKey)
                ? await _localStorageService.GetItemAsync<Dictionary<string, string>>(metadataKey, cancellationToken)
                : new Dictionary<string, string>();

            metadata[key] = value;

            await _localStorageService.SetItemAsync(key, metadata, cancellationToken);
        }
    }
}
