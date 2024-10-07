using CacheLibrary.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace CacheLibrary.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _defaultExpiration;
        public MemoryCacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _defaultExpiration = int.Parse(configuration["CacheSettings:InMemory:DefaultTimeout"] ?? "10");
        }

        public Task<bool> ContainsAsync(string key)
        {
            ValidateKey(key);
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public Task<T?> GetAsync<T>(string key)
        {
            ValidateKey(key);
            T? value = _memoryCache.TryGetValue(key, out T? result) ? result : default;
            return Task.FromResult(value);
        }


        public Task RemoveAsync(string key)
        {
            ValidateKey(key);
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        

        public Task SetAsync<T>(string key, T item)
        {
            return SetAsync<T>(key, item, TimeSpan.FromMinutes(_defaultExpiration), ExpirationType.Absolute);
        }


        public  Task SetAsync<T>(string key, T item, TimeSpan expiration)
        {
            return SetAsync<T>(key, item, expiration, ExpirationType.Absolute);
        }

        public Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType)
        {
            ValidateKey(key);
            var cacheEntryOptions = GetMemoryCacheOption(expiration, expirationType);
            _memoryCache.Set(key, item, cacheEntryOptions);
            return Task.CompletedTask;
        }




        #region Private Methods
        private void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
            }
        }
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
