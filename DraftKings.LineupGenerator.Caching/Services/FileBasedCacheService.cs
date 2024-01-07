using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Caching.Services
{
    public class FileBasedCacheService : ICacheService
    {
        private static SemaphoreSlim _keySemaphore = new SemaphoreSlim(1);

        private static string CacheKeysFilepath => Path.Combine(CacheDirectory, "_keys.json");
        private static string CacheExpirationsFilepath => Path.Combine(CacheDirectory, "_expirations.json");
        private static string CacheDirectory => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "cache");

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            await _keySemaphore.WaitAsync(cancellationToken);

            try
            {
                if (Directory.Exists(CacheDirectory))
                {
                    foreach (var fileInfo in new DirectoryInfo(CacheDirectory).GetFiles())
                    {
                        fileInfo.Delete();
                    }
                }
            }
            finally
            {
                _keySemaphore.Release();
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory, CancellationToken cancellationToken)
            where T : class
        {
            await _keySemaphore.WaitAsync(cancellationToken);

            try
            {
                var fileName = await GetKeyMetadataAsync(key, CacheKeysFilepath, () => $"{Guid.NewGuid()}.json", cancellationToken);
                var absoluteExpiration = await GetKeyMetadataAsync(key, CacheExpirationsFilepath, () => DateTime.UtcNow.Add(expiration).ToString(), cancellationToken);

                var filepath = Path.Combine(CacheDirectory, fileName);

                if (File.Exists(filepath) && DateTime.UtcNow <= DateTime.Parse(absoluteExpiration))
                {
                    var text = await File.ReadAllTextAsync(filepath, cancellationToken);

                    return JsonConvert.DeserializeObject<T>(text);
                }

                if (File.Exists(filepath) && DateTime.UtcNow > DateTime.Parse(absoluteExpiration))
                {
                    absoluteExpiration = DateTime.UtcNow.Add(expiration).ToString();
                }

                var value = await valueFactory();
                var json = JsonConvert.SerializeObject(value);
                await File.WriteAllTextAsync(filepath, json, cancellationToken);

                await WriteKeyMetadataAsync(key, CacheExpirationsFilepath, absoluteExpiration.ToString(), cancellationToken);

                return value;
            }
            finally
            {
                _keySemaphore.Release();
            }
        }

        private async Task<string> GetKeyMetadataAsync(string key, string metadataFilepath, Func<string> metadataValueFactory, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }

            if (!File.Exists(metadataFilepath))
            {
                await File.WriteAllTextAsync(
                    metadataFilepath,
                    JsonConvert.SerializeObject(new Dictionary<string, object>()),
                    cancellationToken);
            }

            var text = await File.ReadAllTextAsync(metadataFilepath, cancellationToken);

            var keysLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            if (!keysLookup.ContainsKey(key))
            {
                keysLookup[key] = metadataValueFactory();

                var json = JsonConvert.SerializeObject(keysLookup);

                await File.WriteAllTextAsync(metadataFilepath, json, cancellationToken);
            }

            return keysLookup[key];
        }

        private async Task WriteKeyMetadataAsync(string key, string metadataFilepath, string value, CancellationToken cancellationToken)
        {
            var text = await File.ReadAllTextAsync(metadataFilepath, cancellationToken);
            var keysLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            keysLookup[key] = value;

            var json = JsonConvert.SerializeObject(keysLookup);

            await File.WriteAllTextAsync(metadataFilepath, json, cancellationToken);
        }
    }
}
