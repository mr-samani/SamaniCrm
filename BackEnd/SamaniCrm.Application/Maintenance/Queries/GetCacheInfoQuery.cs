using MediatR;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Maintenance.Queries;

public record GetCacheInfoQuery(string key):IRequest<object?>
{
}

public class GetCacheInfoQueryHanlder : IRequestHandler<GetCacheInfoQuery, object?>
{
    private readonly ICacheService _cacheService;

    public GetCacheInfoQueryHanlder(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<object?> Handle(GetCacheInfoQuery request, CancellationToken cancellationToken)
    {
        var result = await _cacheService.GetAsync<object>(request.key);
        return result;
    }
}