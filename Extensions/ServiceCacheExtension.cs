using CacheLibrary.Interfaces;
using CacheLibrary.Services;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CacheLibrary.Extensions
{
    public static class ServiceCacheExtension
    {
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
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {

            string redisConnectionString = configuration["CacheSettings:Redis:ConnectionString"] ?? "localhost:6379";
            services.AddSingleton((IConnectionMultiplexer)ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            return services; 
        }
        public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();

            return services; 
        }
    }
}
