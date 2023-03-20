using System;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Caching
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory) where T : class;
    }
}
