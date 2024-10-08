using CacheLibrary.Helper;
using CacheLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheLibrary.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly int _defaultExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer to connect to the Redis server.</param>
        /// <param name="configuration">The application configuration to read settings from.</param>
        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _database = connectionMultiplexer.GetDatabase();
            _defaultExpiration = int.Parse(configuration["CacheSettings:Redis:DefaultTimeout"] ?? "10");
        }

        /// <summary>
        /// Checks if a specified key exists in the Redis cache.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the key exists.</returns>
        public async Task<bool> ContainsAsync(string key)
        {
            CacheHelper.ValidateKey(key);
            return await _database.KeyExistsAsync(key);
        }

        /// <summary>
        /// Retrieves a value associated with the specified key from the Redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key associated with the value.</param>
        /// <returns>A task that represents the asynchronous operation, containing the value associated with the key, or default if the key does not exist.</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            CacheHelper.ValidateKey(key);
            var value = await _database.StringGetAsync(key);
            return value.IsNull ? default : JsonSerializer.Deserialize<T>(value.ToString());
        }

        /// <summary>
        /// Removes the specified key and its associated value from the Redis cache.
        /// </summary>
        /// <param name="key">The key to remove from the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveAsync(string key)
        {
            CacheHelper.ValidateKey(key);
            await _database.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Sets a value in the Redis cache with a default expiration time.
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
        /// Sets a value in the Redis cache with a specified expiration time.
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
        /// Sets a value in the Redis cache with a specified expiration time and type.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="item">The value to store.</param>
        /// <param name="expiration">The expiration time for the value.</param>
        /// <param name="expirationType">The type of expiration (absolute or sliding). Redis does not support sliding</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the expiration type is not recognized.</exception>
        public async Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
        {
            CacheHelper.ValidateKey(key);
            var value = JsonSerializer.Serialize(item);
            switch (expirationType)
            {
                case ExpirationType.Absolute:
                case ExpirationType.Sliding:
                    await _database.StringSetAsync(key, value, expiration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expirationType), expirationType, null);
            }
        }
    }
}
