using CacheLibrary.Interfaces;
using CacheLibrary.Services;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CacheLibrary.Extensions
{
    /// <summary>
    /// Extension methods for configuring caching services.
    /// </summary>
    public static class ServiceCacheExtension
    {
        /// <summary>
        /// Adds Memcached caching services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to which the caching services are added.</param>
        /// <param name="configuration">The application configuration for retrieving Memcached settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// This method configures the Memcached server address and port from the specified configuration.
        /// The default address is "127.0.0.1" and the default port is "11211".
        /// </remarks>
        public static IServiceCollection AddMemcachedCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var memcachedServerAddress = configuration["CacheSettings:Memcached:ServerAddress"] ?? "127.0.0.1";
            var memcachedServerAddressPort = int.Parse(configuration["CacheSettings:Memcached:ServerPort"] ?? "11211");
            services.AddEnyimMemcached(memcachedClientOptions => {
                memcachedClientOptions.Servers.Add(new Server
                {
                    Address = memcachedServerAddress,
                    Port = memcachedServerAddressPort,
                });
            });
            services.AddSingleton<IMemcachedCacheService, MemcachedCacheService>();

            return services;
        }

        /// <summary>
        /// Adds Redis caching services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to which the caching services are added.</param>
        /// <param name="configuration">The application configuration for retrieving Redis settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// This method configures the Redis connection string from the specified configuration.
        /// The default connection string is "localhost:6379".
        /// </remarks>
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            string redisConnectionString = configuration["CacheSettings:Redis:ConnectionString"] ?? "localhost:6379";
            services.AddSingleton((IConnectionMultiplexer)ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            return services;
        }

        /// <summary>
        /// Adds in-memory caching services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to which the caching services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// This method configures the in-memory caching services using ASP.NET Core's built-in memory cache.
        /// </remarks>
        public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();

            return services;
        }
    }
}
