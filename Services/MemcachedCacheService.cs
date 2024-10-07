using CacheLibrary.Interfaces;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Configuration;

namespace CacheLibrary.Services
{
    public class MemcachedCacheService : IMemcachedCacheService
    {
        private readonly IMemcachedClient _memcachedClient;
        private readonly int _defaultExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheService"/> class.
        /// </summary>
        /// <param name="memcachedClient">The Memcached client used for caching operations.</param>
        /// <param name="configuration">The application configuration to read settings from.</param>
        public MemcachedCacheService(IMemcachedClient memcachedClient, IConfiguration configuration)
        {
            _memcachedClient = memcachedClient;
            _defaultExpiration = int.Parse(configuration["CacheSettings:Memcached:DefaultTimeout"] ?? "10");
        }

        /// <summary>
        /// Checks if a specified key exists in the Memcached store.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the key exists.</returns>
        public async Task<bool> ContainsAsync(string key)
        {
            var value = await _memcachedClient.GetAsync<object>(key);
            return value != null;
        }

        /// <summary>
        /// Retrieves a value associated with the specified key from the Memcached store.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key associated with the value.</param>
        /// <returns>A task that represents the asynchronous operation, containing the value associated with the key, or default if the key does not exist.</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            var result = await _memcachedClient.GetAsync<T>(key);
            return result.HasValue ? result.Value : default;
        }

        /// <summary>
        /// Removes the specified key and its associated value from the Memcached store.
        /// </summary>
        /// <param name="key">The key to remove from the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveAsync(string key)
        {
            await _memcachedClient.RemoveAsync(key);
        }

        /// <summary>
        /// Sets a value in the Memcached store with a default expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetAsync<T>(string key, T item)
        {
            await SetAsync(key, item, TimeSpan.FromMinutes(_defaultExpiration), ExpirationType.Absolute);
        }

        /// <summary>
        /// Sets a value in the Memcached store with a specified expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetAsync<T>(string key, T item, TimeSpan expiration)
        {
            await SetAsync(key, item, expiration, ExpirationType.Absolute);
        }

        /// <summary>
        /// Sets a value in the Memcached store with a specified expiration time and type.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <param name="expirationType">The type of expiration (absolute or sliding). Memcached does not support sliding</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the expiration type is not recognized.</exception>
        public async Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
        {
            switch (expirationType)
            {
                case ExpirationType.Absolute:
                case ExpirationType.Sliding:
                    await _memcachedClient.StoreAsync(StoreMode.Set, key, item, expiration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expirationType), expirationType, null);
            }
        }
    }
}
