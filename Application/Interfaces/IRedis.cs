using Microsoft.Extensions.Caching.Distributed;

namespace Application
{
    public interface IRedis
    {
        Task<TValue?> LoadAsync<TValue>(RedisKey keyType, params object[] keyIds)
            where TValue : class;

        Task<TValue?> LoadOrCreateAsync<TValue>(RedisKey keyType, Func<Task<(TValue Value, DistributedCacheEntryOptions EntryOptions)>> loadAction, params object[] keyIds)
            where TValue : class;

        Task AddOrUpdateAsync<TValue>(RedisKey keyType, TValue value, DistributedCacheEntryOptions entryOptions, params object[] keyIds)
            where TValue : class;

        Task UpdateAsync<TValue>(RedisKey keyType, TValue value, params object[] keyIds)
            where TValue : class;

        Task DeleteAsync(RedisKey keyType, params object[] keyIds);
    }
}