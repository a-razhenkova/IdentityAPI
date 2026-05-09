using Application;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure
{
    public class RedisService : IRedis
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IDistributedCache distributedCache, ILogger<RedisService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<TValue?> LoadAsync<TValue>(RedisKey keyType, object[] keyIds, CancellationToken cancellationToken = default)
            where TValue : class
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            return await LoadAsync<TValue>(key, canThrowException: true, cancellationToken);
        }

        public async Task<TValue?> LoadOrCreateAsync<TValue>(RedisKey keyType, object[] keyIds, Func<CancellationToken, Task<(TValue Value, DistributedCacheEntryOptions EntryOptions)>> loadCallback, CancellationToken cancellationToken = default)
            where TValue : class
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            TValue? value = await LoadAsync<TValue>(key, canThrowException: false, cancellationToken);

            if (value is null)
            {
                (value, DistributedCacheEntryOptions entryOptions) = await loadCallback(cancellationToken);

                // if entryOptions is null => the value should not be cached
                if (value is not null && entryOptions is not null)
                {
                    await AddOrUpdateAsync(key, value, entryOptions, canThrowException: false, cancellationToken);
                }
            }

            return value;
        }

        public async Task AddOrUpdateAsync<TValue>(RedisKey keyType, TValue value, DistributedCacheEntryOptions entryOptions, object[] keyIds, CancellationToken cancellationToken = default)
            where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            await AddOrUpdateAsync(key, value, entryOptions, canThrowException: true, cancellationToken);
        }

        public async Task UpdateAsync<TValue>(RedisKey keyType, TValue value, object[] keyIds, CancellationToken cancellationToken = default)
           where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            string valueAsString = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, valueAsString, cancellationToken);
        }

        public async Task DeleteAsync(RedisKey keyType, object[] keyIds, CancellationToken cancellationToken = default)
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        private async Task<TValue?> LoadAsync<TValue>(string key, bool canThrowException = true, CancellationToken cancellationToken = default)
            where TValue : class
        {
            TValue? value = null;

            try
            {
                string? valueAsString = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrWhiteSpace(valueAsString))
                {
                    value = JsonSerializer.Deserialize<TValue>(valueAsString);
                }
            }
            catch (Exception еxception)
            {
                _logger.LogCritical(еxception, еxception.Message);

                if (canThrowException)
                    throw;
            }

            return value;
        }

        private async Task AddOrUpdateAsync<TValue>(string key, TValue value, DistributedCacheEntryOptions entryOptions, bool canThrowException = true, CancellationToken cancellationToken = default)
            where TValue : class
        {
            try
            {
                if (value is null)
                    throw new InvalidOperationException();

                string valueAsString = JsonSerializer.Serialize(value);

                await _distributedCache.SetStringAsync(key, valueAsString, entryOptions, cancellationToken);
            }
            catch (Exception еxception)
            {
                _logger.LogCritical(еxception, еxception.Message);

                if (canThrowException)
                    throw;
            }
        }
    }
}