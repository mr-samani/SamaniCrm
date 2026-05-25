using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetLogStatesQuery : IRequest<LogStatsDto>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; } = DateTime.MinValue;

    public Guid? TenantId { get; set; }
}


public class GetLogStatesQueryHandler : IRequestHandler<GetLogStatesQuery, LogStatsDto>
{
    private readonly ILogService _logService;

    public GetLogStatesQueryHandler(ILogService logService)
    {
        _logService = logService;
    }

    public async Task<LogStatsDto> Handle(GetLogStatesQuery request, CancellationToken cancellationToken)
    {
        var result = await _logService.GetStats(request.FromDate, request.ToDate, request.TenantId, cancellationToken);
        return result;
    }
}