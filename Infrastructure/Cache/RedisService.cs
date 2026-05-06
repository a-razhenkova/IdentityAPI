using Application;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shared;
using System.Text.Json;

namespace Infrastructure
{
    /// <summary>
    /// Provides Redis cache operations.
    /// </summary>
    public class RedisService : IRedis
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IDistributedCache distributedCache, ILogger<RedisService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        /// <summary>
        /// Loads a value from Redis cache by key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <param name="keyType">The Redis key type.</param>
        /// <param name="keyIds">The key identifiers used to build the cache key.</param>
        /// <returns>The cached value if found, otherwise - null.</returns>
        public async Task<TValue?> LoadAsync<TValue>(RedisKey keyType, params object[] keyIds)
            where TValue : class
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            return await LoadAsync<TValue>(key);
        }

        /// <summary>
        /// Loads a value from Redis cache by key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <param name="key">The Redis cache key.</param>
        /// <param name="canThrowException">
        /// If true, exceptions encountered during cache access or deserialization will be thrown.
        /// Otherwise, exceptions are logged and the method returns null.
        /// </param>
        /// <returns>The cached value if found, otherwise null.</returns>
        private async Task<TValue?> LoadAsync<TValue>(string key, bool canThrowException = true)
            where TValue : class
        {
            TValue? value = null;

            try
            {
                string? valueAsString = await _distributedCache.GetStringAsync(key);
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

        /// <summary>
        /// Loads a value from Redis cache or creates and caches it if not found.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load or create.</typeparam>
        /// <param name="keyType">The Redis key type.</param>
        /// <param name="loadAction">The function to load and provide the value and cache options if not found.</param>
        /// <param name="keyIds">The key identifiers used to build the cache key.</param>
        /// <returns>The cached or newly created value.</returns>
        public async Task<TValue?> LoadOrCreateAsync<TValue>(RedisKey keyType, Func<Task<(TValue Value, DistributedCacheEntryOptions EntryOptions)>> loadAction, params object[] keyIds)
            where TValue : class
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            TValue? value = await LoadAsync<TValue>(key, canThrowException: false);

            if (value is null)
            {
                (value, DistributedCacheEntryOptions entryOptions) = await loadAction();

                // if entryOptions is null => the value should not be cached
                if (value is not null && entryOptions is not null)
                {
                    await AddOrUpdateAsync(key, value, entryOptions, canThrowException: false);
                }
            }

            return value;
        }

        /// <summary>
        /// Adds or updates a value in Redis cache.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to store.</typeparam>
        /// <param name="keyType">The Redis key type.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="entryOptions">The cache entry options.</param>
        /// <param name="keyIds">The key identifiers used to build the cache key.</param>
        public async Task AddOrUpdateAsync<TValue>(RedisKey keyType, TValue value, DistributedCacheEntryOptions entryOptions, params object[] keyIds)
            where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            await AddOrUpdateAsync(key, value, entryOptions);
        }

        /// <summary>
        /// Adds or updates a value in Redis cache.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to store.</typeparam>
        /// <param name="key">The Redis cache key.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="entryOptions">The cache entry options.</param>
        /// <param name="canThrowException">
        /// If true, exceptions encountered during cache access or serialization will be thrown.
        /// Otherwise, exceptions are logged and the method returns without throwing.
        /// </param>
        private async Task AddOrUpdateAsync<TValue>(string key, TValue value, DistributedCacheEntryOptions entryOptions, bool canThrowException = true)
           where TValue : class
        {
            try
            {
                if (value is null)
                    throw new InvalidOperationException();

                string valueAsString = JsonSerializer.Serialize(value);

                await _distributedCache.SetStringAsync(key, valueAsString, entryOptions);
            }
            catch (Exception еxception)
            {
                _logger.LogCritical(еxception, еxception.Message);

                if (canThrowException)
                    throw;
            }
        }

        /// <summary>
        /// Updates a value in Redis cache.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to store.</typeparam>
        /// <param name="keyType">The Redis key type.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="keyIds">The key identifiers used to build the cache key.</param>
        public async Task UpdateAsync<TValue>(RedisKey keyType, TValue value, params object[] keyIds)
           where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            string valueAsString = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, valueAsString);
        }

        /// <summary>
        /// Deletes a value from Redis cache by key.
        /// </summary>
        /// <param name="keyType">The Redis key type.</param>
        /// <param name="keyIds">The key identifiers used to build the cache key.</param>
        public async Task DeleteAsync(RedisKey keyType, params object[] keyIds)
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            await _distributedCache.RemoveAsync(key);
        }
    }
}