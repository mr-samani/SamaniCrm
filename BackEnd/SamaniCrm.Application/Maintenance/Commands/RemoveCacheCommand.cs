using MediatR;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static SamaniCrm.Core.Shared.Consts.AppPermissions.Maintenance;

namespace SamaniCrm.Application.Maintenance.Commands;

public record RemoveCacheCommand(string key):IRequest<bool>
{
}
public class RemoveCacheCommandHandler : IRequestHandler<RemoveCacheCommand, bool>
{
    private readonly ICacheService _cacheService;

    public RemoveCacheCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(RemoveCacheCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(request.key);
        return true;
    }
}