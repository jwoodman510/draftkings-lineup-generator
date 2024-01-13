using System;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Caching
{
    public interface ICacheService
    {
        Task ClearAsync(CancellationToken cancellationToken);

        Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> valueFactory, CancellationToken cancellationToken) where T : class;
    }
}
