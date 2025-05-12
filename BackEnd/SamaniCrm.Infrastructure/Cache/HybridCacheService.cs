using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using StackExchange.Redis;

namespace SamaniCrm.Infrastructure.Cache
{
    public class HybridCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDatabase _redisDb;
        private readonly HashSet<string> _keys = new();
        private readonly object _lock = new();

        public HybridCacheService(IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _memoryCache = memoryCache;
            _redisDb = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out var memValue))
            {
                return memValue is T t ? t : default;
            }

            var redisValue = await _redisDb.StringGetAsync(key);
            if (redisValue.IsNullOrEmpty) return default;

            var value = JsonSerializer.Deserialize<T>(redisValue!);
            _memoryCache.Set(key, value); // optional: you can add expiration here
            return value;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);

            var memOptions = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                memOptions.SetAbsoluteExpiration(expiration.Value);

            _memoryCache.Set(key, value, memOptions);
            await _redisDb.StringSetAsync(key, json, expiration);

            lock (_lock)
            {
                _keys.Add(key);
            }
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            _redisDb.KeyDelete(key);

            lock (_lock)
            {
                _keys.Remove(key);
            }

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string? pattern = null)
        {
            // Redis does not support full scan directly from IDatabase
            var keys = _keys.ToList();
            if (!string.IsNullOrEmpty(pattern))
                keys = keys.Where(k => k.Contains(pattern)).ToList();

            return await Task.FromResult(keys);
        }

        public async Task ClearAsync()
        {
            lock (_lock)
            {
                foreach (var key in _keys.ToList())
                {
                    _memoryCache.Remove(key);
                    _redisDb.KeyDelete(key);
                }

                _keys.Clear();
            }

            await Task.CompletedTask;
        }

        public async Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            if (await _redisDb.KeyExistsAsync(key))
            {
                var ttl = await _redisDb.KeyTimeToLiveAsync(key);
                var value = await _redisDb.StringGetAsync(key);
                var size = Encoding.UTF8.GetByteCount(value);

                return new CacheEntryDto
                {
                    Key = key,
                    Provider = "Hybrid",
                    Expiration = ttl,
                    SizeInBytes = size,
                    LastModified = null // Redis doesn't store this natively
                };
            }

            return null;
        }
    }
}
