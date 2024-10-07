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

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _database = connectionMultiplexer.GetDatabase();
            _defaultExpiration = int.Parse(configuration["CacheSettings:Redis:DefaultTimeout"] ?? "10");
        }

        public async Task<bool> ContainsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.IsNull ? default : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T item)
        {
            await SetAsync(key, item, TimeSpan.FromMinutes(_defaultExpiration), ExpirationType.Absolute);
        }

        public async Task SetAsync<T>(string key, T item, TimeSpan expiration)
        {
            await SetAsync(key, item, expiration, ExpirationType.Absolute);
        }

        public async Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
        {
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
