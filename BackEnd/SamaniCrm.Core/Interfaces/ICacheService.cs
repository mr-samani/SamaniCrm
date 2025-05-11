using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.DTOs;

namespace SamaniCrm.Core.Shared.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<IEnumerable<string>> GetKeysAsync(string? pattern = null);
        Task ClearAsync();

        Task<CacheEntryDto?> GetMetaAsync(string key);
    }

}
