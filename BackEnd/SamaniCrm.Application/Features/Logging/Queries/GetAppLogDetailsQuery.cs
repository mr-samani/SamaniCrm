using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public record GetAppLogDetailsQuery(Guid? tenantId, long logId):IRequest<AppLogEntryDto>;


public class GetAppLogDetailsQueryHandlder : IRequestHandler<GetAppLogDetailsQuery, AppLogEntryDto>
{
    private readonly IAppLogService _logService;

    public GetAppLogDetailsQueryHandlder(IAppLogService logService)
    {
        _logService = logService;
    }

    async Task<AppLogEntryDto> IRequestHandler<GetAppLogDetailsQuery, AppLogEntryDto>.Handle(GetAppLogDetailsQuery request, CancellationToken cancellation)
    {
        var result = await _logService.GetLogDetails(request, cancellation);
        return result;
    }
}
