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

        private string GetPath(string key) => Path.Combine(_basePath, $"{key}.json");

        public async Task<T?> GetAsync<T>(string key)
        {
            var path = GetPath(key);
            if (!File.Exists(path)) return default;

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var path = GetPath(key);
            var json = JsonSerializer.Serialize(value);
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



        public Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            var path = GetPath(key);
            if (!File.Exists(path)) return Task.FromResult<CacheEntryDto?>(null);

            var info = new FileInfo(path);
            return Task.FromResult<CacheEntryDto?>(new CacheEntryDto
            {
                Key = key,
                Provider = "File",
                SizeInBytes = info.Length,
                LastModified = info.LastWriteTime
            });
        }



    }

}
