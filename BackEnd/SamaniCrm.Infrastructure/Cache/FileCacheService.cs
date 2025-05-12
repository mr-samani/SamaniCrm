using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;


namespace SamaniCrm.Infrastructure.Cache
{

    public class FileCacheService : ICacheService
    {

        private readonly string _basePath;

        public FileCacheService(IOptions<CacheSettings> settings)
        {
            _basePath = settings.Value.File.BasePath;
            Directory.CreateDirectory(_basePath);
        }



        private class CacheWrapper<T>
        {
            public T Value { get; set; } = default!;
            public DateTime? ExpiresAt { get; set; }
        }



        private string GetPath(string key) => Path.Combine(_basePath, $"{key}.json");

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var path = GetPath(key);
                if (!File.Exists(path)) return default;

                var json = await File.ReadAllTextAsync(path);
                var wrapper = JsonSerializer.Deserialize<CacheWrapper<T>>(json);

                if (wrapper == null) return default;

                if (wrapper.ExpiresAt.HasValue && wrapper.ExpiresAt.Value < DateTime.UtcNow)
                {
                    File.Delete(path); // delete expired file
                    return default;
                }


                return wrapper.Value;
            }
            catch
            {
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var path = GetPath(key);
            var wrapper = new CacheWrapper<T>
            {
                Value = value,
                ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
            };
            var json = JsonSerializer.Serialize(wrapper);
            await File.WriteAllTextAsync(path, json);
        }

        public Task RemoveAsync(string key)
        {
            var path = GetPath(key);
            if (File.Exists(path)) File.Delete(path);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetKeysAsync(string? pattern = null)
        {
            var files = Directory.GetFiles(_basePath, "*.json");
            var keys = files.Select(Path.GetFileNameWithoutExtension);

            if (!string.IsNullOrEmpty(pattern))
                keys = keys.Where(k => k.Contains(pattern));

            return Task.FromResult(keys);
        }

        public Task ClearAsync()
        {
            foreach (var file in Directory.GetFiles(_basePath))
            {
                File.Delete(file);
            }

            return Task.CompletedTask;
        }



        public async Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            var path = GetPath(key);
            if (!File.Exists(path)) return null;

            var json = await File.ReadAllTextAsync(path);
            var document = JsonDocument.Parse(json);

            DateTime? expiresAt = null;
            if (document.RootElement.TryGetProperty("ExpiresAt", out var expiresProp) &&
                expiresProp.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(expiresProp.GetString(), out var dt))
            {
                expiresAt = dt;
            }

            TimeSpan? timeToLive = expiresAt.HasValue
                ? (expiresAt > DateTime.UtcNow ? expiresAt.Value - DateTime.UtcNow : null)
                : null;

            var info = new FileInfo(path);

            return new CacheEntryDto
            {
                Key = key,
                Provider = "File",
                SizeInBytes = info.Length,
                LastModified = info.LastWriteTime,
                Expiration = timeToLive
            };
        }


    }

}
