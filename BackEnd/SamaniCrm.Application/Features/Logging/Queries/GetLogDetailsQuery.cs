using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public record GetLogDetailsQuery(Guid? tenantId, long logId):IRequest<LogEntryDto>;


public class GetLogDetailsQueryHandlder : IRequestHandler<GetLogDetailsQuery, LogEntryDto>
{
    private readonly ILogService _logService;

    public GetLogDetailsQueryHandlder(ILogService logService)
    {
        _logService = logService;
    }

    async Task<LogEntryDto> IRequestHandler<GetLogDetailsQuery, LogEntryDto>.Handle(GetLogDetailsQuery request, CancellationToken cancellation)
    {
        var result = await _logService.GetLogDetails(request, cancellation);
        return result;
    }
}
