using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using StackExchange.Redis;

namespace SamaniCrm.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly IConnectionMultiplexer _connection;

        public RedisCacheService(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _database = connection.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public Task<IEnumerable<string>> GetKeysAsync(string? pattern)
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            return (Task<IEnumerable<string>>)server.Keys(pattern: pattern ?? "*").Select(k => k.ToString());
        }

        public async Task ClearAsync()
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            foreach (var key in server.Keys())
            {
                await _database.KeyDeleteAsync(key);
            }
        }



        public async Task<CacheEntryDto?> GetMetaAsync(string key)
        {
            if (!await _database.KeyExistsAsync(key)) return null;

            var ttl = await _database.KeyTimeToLiveAsync(key);
            var size = await _database.StringLengthAsync(key);

            return new CacheEntryDto
            {
                Key = key,
                Provider = "Redis",
                Expiration = ttl,
                SizeInBytes = size,
                LastModified = null // Redis به صورت پیش فرض تاریخ ندارد
            };
        }
 
    }

}
