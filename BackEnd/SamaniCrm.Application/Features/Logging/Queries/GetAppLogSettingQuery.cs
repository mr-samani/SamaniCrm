using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public record GetAppLogSettingQuery(Guid? tenantId) : IRequest<TenantAppLogSettingDto>
{
}

public class GetAppLogSettingQueryHandler : IRequestHandler<GetAppLogSettingQuery, TenantAppLogSettingDto>
{

    private readonly IAppLogConfigurationService _configService;

    public GetAppLogSettingQueryHandler(IAppLogConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task<TenantAppLogSettingDto> Handle(GetAppLogSettingQuery request, CancellationToken cancellation)
    {
        var result = await _configService.GetSettingAsync(request.tenantId, cancellation);
        if (result == null)
        {
            throw new NotFoundException("Setting not found!");
        }

        return result;
    }
}



