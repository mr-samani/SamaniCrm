using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Infrastructure.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _keys = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key) =>
            Task.FromResult(_cache.TryGetValue(key, out var value) ? (T?)value : default);

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            _cache.Set(key, value, options);
            _keys.Add(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _keys.Remove(key);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetKeysAsync(string? pattern = null)
        {
            var filtered = string.IsNullOrEmpty(pattern)
                ? _keys
                : _keys.Where(k => k.Contains(pattern)).ToList();

            return Task.FromResult((IEnumerable<string>)filtered);
        }

        public Task ClearAsync()
        {
            foreach (var key in _keys.ToList())
                _cache.Remove(key);

            _keys.Clear();
            return Task.CompletedTask;
        }



        public Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                var size = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(value));
                return Task.FromResult<CacheEntryDto?>(new CacheEntryDto
                {
                    Key = key,
                    Provider = "Memory",
                    SizeInBytes = size,
                    Expiration = null,
                    LastModified = null
                });
            }

            return Task.FromResult<CacheEntryDto?>(null);
        }
    }

}
