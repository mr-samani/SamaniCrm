using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Core.Shared.Interfaces;
using StackExchange.Redis;

namespace SamaniCrm.Infrastructure.Cache
{
    public static class CacheServiceFactory
    {
        public static void AddCacheService(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("CacheSettings").Get<CacheSettings>();

            services.Configure<CacheSettings>(config.GetSection("CacheSettings"));

            switch (settings.Provider)
            {
                case "Redis":
                    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(settings.Redis.ConnectionString));
                    services.AddScoped<ICacheService, RedisCacheService>();
                    break;

                case "File":
                    services.AddScoped<ICacheService, FileCacheService>();
                    break;

                case "Memory":
                default:
                    services.AddMemoryCache();
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
            }
        }
    }

}
