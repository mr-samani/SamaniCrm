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
        private readonly HashSet<string> _keys = new();
        private readonly object _lock = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // Wrapper class to store value + metadata
        private class CacheItemWrapper<T>
        {
            public T Value { get; set; } = default!;
            public DateTime CreatedTime { get; set; }
            public TimeSpan? Expiration { get; set; }

            public DateTime? GetExpiresAt()
            {
                return Expiration.HasValue ? CreatedTime.Add(Expiration.Value) : null;
            }

            public TimeSpan? GetTimeToLive()
            {
                var expiresAt = GetExpiresAt();
                return expiresAt.HasValue ? expiresAt.Value - DateTime.UtcNow : null;
            }
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var wrapperObj) && wrapperObj is CacheItemWrapper<T> wrapper)
            {
                return Task.FromResult<T?>(wrapper.Value);
            }

            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            var wrapper = new CacheItemWrapper<T>
            {
                Value = value,
                CreatedTime = DateTime.UtcNow,
                Expiration = expiration
            };

            _cache.Set(key, wrapper, options);

            lock (_lock)
            {
                _keys.Add(key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            lock (_lock)
            {
                _keys.Remove(key);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetKeysAsync(string? pattern = null)
        {
            IEnumerable<string> filtered;
            lock (_lock)
            {
                filtered = string.IsNullOrEmpty(pattern)
                    ? _keys.ToList()
                    : _keys.Where(k => k.Contains(pattern)).ToList();
            }

            return Task.FromResult(filtered);
        }

        public Task ClearAsync()
        {
            lock (_lock)
            {
                foreach (var key in _keys.ToList())
                {
                    _cache.Remove(key);
                }

                _keys.Clear();
            }

            return Task.CompletedTask;
        }

        public Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            if (_cache.TryGetValue(key, out var wrapperObj))
            {
                var json = JsonSerializer.Serialize(wrapperObj);
                var size = Encoding.UTF8.GetByteCount(json);

                DateTime? lastModified = null;
                TimeSpan? ttl = null;

                if (wrapperObj?.GetType().IsGenericType == true &&
                    wrapperObj.GetType().GetGenericTypeDefinition() == typeof(CacheItemWrapper<>))
                {
                    dynamic dynamicWrapper = wrapperObj;
                    lastModified = (DateTime)dynamicWrapper.CreatedAt;
                    ttl = dynamicWrapper.GetTimeToLive();
                }

                return Task.FromResult<CacheEntryDto?>(new CacheEntryDto
                {
                    Key = key,
                    Provider = "Memory",
                    SizeInBytes = size,
                    LastModified = lastModified,
                    Expiration = ttl
                });
            }

            return Task.FromResult<CacheEntryDto?>(null);
        }
    }









}
