using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public record GetLogSettingQuery(Guid? tenantId) : IRequest<TenantLogSettingDto>
{
}

public class GetLogSettingQueryHandler : IRequestHandler<GetLogSettingQuery, TenantLogSettingDto>
{

    private readonly ILogConfigurationService _configService;

    public GetLogSettingQueryHandler(ILogConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task<TenantLogSettingDto> Handle(GetLogSettingQuery request, CancellationToken cancellation)
    {
        var result = await _configService.GetSettingAsync(request.tenantId, cancellation);
        if (result == null)
        {
            throw new NotFoundException("Setting not found!");
        }

        return result;
    }
}



