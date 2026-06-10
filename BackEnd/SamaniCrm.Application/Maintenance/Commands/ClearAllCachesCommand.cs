using MediatR;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static SamaniCrm.Core.Shared.Consts.AppPermissions.Maintenance;

namespace SamaniCrm.Application.Maintenance.Commands;

public class ClearAllCachesCommand:IRequest<bool>
{
}
public class ClearAllCachesCommandHandler : IRequestHandler<ClearAllCachesCommand, bool>
{
    private readonly ICacheService _cacheService;

    public ClearAllCachesCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(ClearAllCachesCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.ClearAsync();
        return true;
    }
}