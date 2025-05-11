using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;


namespace SamaniCrm.Application.Maintenance.Queries
{
    public class GetAllCacheEntriesQueryHandler : IRequestHandler<GetAllCacheEntriesQuery, List<CacheEntryDto>>
    {
        private readonly ICacheService _cache;

        public GetAllCacheEntriesQueryHandler(ICacheService cache) => _cache = cache;

        public async Task<List<CacheEntryDto>> Handle(GetAllCacheEntriesQuery request, CancellationToken cancellationToken)
        {
            var keys = await _cache.GetKeysAsync(request.Pattern);
            var result = new List<CacheEntryDto>();

            foreach (var key in keys)
            {
                var meta = await _cache.GetMetaAsync(key);
                if (meta != null)
                    result.Add(meta);
            }

            return result;
        }
    }

}
