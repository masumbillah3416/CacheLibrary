using System;
using System.Threading.Tasks;

namespace CacheLibrary.Interfaces
{
    public interface ICacheBase
    {
        // Add an item to the cache
        Task SetAsync<T>(string key, T item);
        Task SetAsync<T>(string key, T item, TimeSpan expiration);
        Task SetAsync<T>(string key, T item, TimeSpan expiration,ExpirationType expirationType);

        // Get an item from the cache
        Task<T?> GetAsync<T>(string key);

        // Check if an item exists in the cache
        Task<bool> ContainsAsync(string key);

        // Remove an item from the cache
        Task RemoveAsync(string key);
    }
    public enum ExpirationType
    {
        Absolute,
        Sliding
    }
}
