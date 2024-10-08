using CacheLibrary.Helper;
using CacheLibrary.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace CacheLibrary.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _defaultExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
        /// </summary>
        /// <param name="memoryCache">The in-memory cache used for caching operations.</param>
        /// <param name="configuration">The application configuration to read settings from.</param>
        public MemoryCacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _defaultExpiration = int.Parse(configuration["CacheSettings:InMemory:DefaultTimeout"] ?? "10");
        }

        /// <summary>
        /// Checks if a specified key exists in the in-memory cache.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the key exists.</returns>
        public Task<bool> ContainsAsync(string key)
        {
            CacheHelper.ValidateKey(key);
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        /// <summary>
        /// Retrieves a value associated with the specified key from the in-memory cache.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key associated with the value.</param>
        /// <returns>A task that represents the asynchronous operation, containing the value associated with the key, or default if the key does not exist.</returns>
        public Task<T?> GetAsync<T>(string key)
        {
            CacheHelper.ValidateKey(key);
            T? value = _memoryCache.TryGetValue(key, out T? result) ? result : default;
            return Task.FromResult(value);
        }

        /// <summary>
        /// Removes the specified key and its associated value from the in-memory cache.
        /// </summary>
        /// <param name="key">The key to remove from the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task RemoveAsync(string key)
        {
            CacheHelper.ValidateKey(key);
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets a value in the in-memory cache with a default expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SetAsync<T>(string key, T item)
        {
            return SetAsync<T>(key, item, TimeSpan.FromMinutes(_defaultExpiration), ExpirationType.Absolute);
        }

        /// <summary>
        /// Sets a value in the in-memory cache with a specified expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SetAsync<T>(string key, T item, TimeSpan expiration)
        {
            return SetAsync<T>(key, item, expiration, ExpirationType.Absolute);
        }

        /// <summary>
        /// Sets a value in the in-memory cache with a specified expiration time and type.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <param name="expirationType">The type of expiration (absolute or sliding).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the expiration type is not recognized.</exception>
        public Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
        {
            CacheHelper.ValidateKey(key);
            var cacheEntryOptions = GetMemoryCacheOption(expiration, expirationType);
            _memoryCache.Set(key, item, cacheEntryOptions);
            return Task.CompletedTask;
        }

        #region Private Methods


        /// <summary>
        /// Gets the memory cache options based on the expiration time and type.
        /// </summary>
        /// <param name="expiration">The expiration time for the cache entry.</param>
        /// <param name="expirationType">The type of expiration (absolute or sliding).</param>
        /// <returns>The configured <see cref="MemoryCacheEntryOptions"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the expiration type is not recognized.</exception>
        private static MemoryCacheEntryOptions GetMemoryCacheOption(TimeSpan expiration, ExpirationType expirationType)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();

            switch (expirationType)
            {
                case ExpirationType.Absolute:
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = expiration;
                    break;
                case ExpirationType.Sliding:
                    cacheEntryOptions.SlidingExpiration = expiration;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expirationType), expirationType, null);
            }
            return cacheEntryOptions;
        }

        #endregion
    }
}
