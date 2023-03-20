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

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory)
            where T : class
        {
            await _keySemaphore.WaitAsync();

            var fileName = await GetKeyMetadataAsync(key, CacheKeysFilepath, () => $"{Guid.NewGuid()}.json");
            var absoluteExpiration = await GetKeyMetadataAsync(key, CacheExpirationsFilepath, () => DateTime.UtcNow.Add(expiration).ToString());

            var filepath = Path.Combine(CacheDirectory, fileName);

            if (File.Exists(filepath) && DateTime.UtcNow <= DateTime.Parse(absoluteExpiration))
            {
                var text = await File.ReadAllTextAsync(filepath);

                _keySemaphore.Release();

                return JsonConvert.DeserializeObject<T>(text);
            }

            var value = await valueFactory();
            var json = JsonConvert.SerializeObject(value);
            await File.WriteAllTextAsync(filepath, json);

            await WriteKeyMetadataAsync(key, CacheExpirationsFilepath, absoluteExpiration.ToString());

            _keySemaphore.Release();

            return value;
        }

        private async Task<string> GetKeyMetadataAsync(string key, string metadataFilepath, Func<string> metadataValueFactory)
        {
            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }

            if (!File.Exists(metadataFilepath))
            {
                await File.WriteAllTextAsync(
                    metadataFilepath,
                    JsonConvert.SerializeObject(new Dictionary<string, object>()));
            }

            var text = await File.ReadAllTextAsync(metadataFilepath);

            var keysLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            if (!keysLookup.ContainsKey(key))
            {
                keysLookup[key] = metadataValueFactory();

                var json = JsonConvert.SerializeObject(keysLookup);

                await File.WriteAllTextAsync(metadataFilepath, json);
            }

            return keysLookup[key];
        }

        private async Task WriteKeyMetadataAsync(string key, string metadataFilepath, string value)
        {
            var text = await File.ReadAllTextAsync(metadataFilepath);
            var keysLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            keysLookup[key] = value;

            var json = JsonConvert.SerializeObject(keysLookup);

            await File.WriteAllTextAsync(metadataFilepath, json);
        }
    }
}
