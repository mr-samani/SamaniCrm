using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetAppLogStatesQuery : IRequest<AppLogStatsDto>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; } = DateTime.MinValue;

    public Guid? TenantId { get; set; }
}


public class GetAppLogStatesQueryHandler : IRequestHandler<GetAppLogStatesQuery, AppLogStatsDto>
{
    private readonly IAppLogService _logService;

    public GetAppLogStatesQueryHandler(IAppLogService logService)
    {
        _logService = logService;
    }

    public async Task<AppLogStatsDto> Handle(GetAppLogStatesQuery request, CancellationToken cancellationToken)
    {
        var result = await _logService.GetStats(request.FromDate, request.ToDate, request.TenantId, cancellationToken);
        return result;
    }
}