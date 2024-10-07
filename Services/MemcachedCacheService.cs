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

        public MemcachedCacheService(IMemcachedClient memcachedClient, IConfiguration configuration)
        {
            _memcachedClient = memcachedClient;
            _defaultExpiration = int.Parse(configuration["CacheSettings:Memcached:DefaultTimeout"] ?? "10");
        }

        public async Task<bool> ContainsAsync(string key)
        {
            var value =await  _memcachedClient.GetAsync<object>(key);
            return value != null;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var result = await _memcachedClient.GetAsync<T>(key);
            return result.HasValue ? result.Value : default; 
        }

        public async Task RemoveAsync(string key)
        {
            await _memcachedClient.RemoveAsync(key);
        }

        public async Task SetAsync<T>(string key, T item)
        {
            await SetAsync(key, item, TimeSpan.FromMinutes(_defaultExpiration), ExpirationType.Absolute);
        }

        public async Task SetAsync<T>(string key, T item, TimeSpan expiration)
        {
            await SetAsync(key, item, expiration, ExpirationType.Absolute);
        }

        public  async Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
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
