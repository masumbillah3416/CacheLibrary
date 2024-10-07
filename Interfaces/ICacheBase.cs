using System;
using System.Threading.Tasks;

namespace CacheLibrary.Interfaces
{
    /// <summary>
    /// Defines the basic operations for a caching service.
    /// </summary>
    public interface ICacheBase
    {
        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item to cache.</typeparam>
        /// <param name="key">The unique key for the cached item.</param>
        /// <param name="item">The item to cache.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T item);

        /// <summary>
        /// Adds an item to the cache with a specific expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the item to cache.</typeparam>
        /// <param name="key">The unique key for the cached item.</param>
        /// <param name="item">The item to cache.</param>
        /// <param name="expiration">The duration after which the cached item should expire.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T item, TimeSpan expiration);

        /// <summary>
        /// Adds an item to the cache with a specific expiration time and type.
        /// </summary>
        /// <typeparam name="T">The type of the item to cache.</typeparam>
        /// <param name="key">The unique key for the cached item.</param>
        /// <param name="item">The item to cache.</param>
        /// <param name="expiration">The duration after which the cached item should expire.</param>
        /// <param name="expirationType">The type of expiration to apply (absolute or sliding).Redis and Memcached does not support sliding.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T item, TimeSpan expiration, ExpirationType expirationType);

        /// <summary>
        /// Retrieves an item from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The unique key for the cached item.</param>
        /// <returns>The cached item, or <c>null</c> if it does not exist.</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Checks if an item exists in the cache.
        /// </summary>
        /// <param name="key">The unique key for the cached item.</param>
        /// <returns><c>true</c> if the item exists in the cache; otherwise, <c>false</c>.</returns>
        Task<bool> ContainsAsync(string key);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The unique key for the cached item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(string key);
    }

    /// <summary>
    /// Specifies the type of expiration for cached items.
    /// </summary>
    public enum ExpirationType
    {
        /// <summary>
        /// The cached item will expire after a fixed duration.
        /// </summary>
        Absolute,

        /// <summary>
        /// The cached item will expire if it is not accessed within a certain duration.
        /// Redis and Memcached does not support this.
        /// </summary>
        Sliding
    }
}
