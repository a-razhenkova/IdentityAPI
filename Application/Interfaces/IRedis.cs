using Microsoft.Extensions.Caching.Distributed;

namespace Application
{
    public interface IRedis
    {
        Task<TValue?> LoadAsync<TValue>(RedisKey keyType, object[] keyIds, CancellationToken cancellationToken = default)
            where TValue : class;

        Task<TValue?> LoadOrCreateAsync<TValue>(RedisKey keyType, object[] keyIds, Func<CancellationToken, Task<(TValue Value, DistributedCacheEntryOptions EntryOptions)>> loadCallback, CancellationToken cancellationToken = default)
            where TValue : class;

        Task AddOrUpdateAsync<TValue>(RedisKey keyType, TValue value, DistributedCacheEntryOptions entryOptions, object[] keyIds, CancellationToken cancellationToken = default)
            where TValue : class;

        Task UpdateAsync<TValue>(RedisKey keyType, TValue value, object[] keyIds, CancellationToken cancellationToken = default)
            where TValue : class;

        Task DeleteAsync(RedisKey keyType, object[] keyIds, CancellationToken cancellationToken = default);
    }
}