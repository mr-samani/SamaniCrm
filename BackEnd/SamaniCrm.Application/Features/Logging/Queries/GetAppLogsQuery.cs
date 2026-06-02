using MediatR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetAppLogsQuery : PaginationRequest, IRequest<PaginatedResult<AppLogEntryDto>>
{
    public Guid? TenantId { get; set; }
    public List<LogLevel>? Levels { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? UserId { get; set; }
    public string? Search { get; set; }
}

public class GetAppLogsQueryHandler : IRequestHandler<GetAppLogsQuery, PaginatedResult<AppLogEntryDto>>
{
    private readonly IAppLogService _logService;

    public GetAppLogsQueryHandler(IAppLogService logService)
    {
        _logService = logService;
    }

    public async Task<PaginatedResult<AppLogEntryDto>> Handle(GetAppLogsQuery request, CancellationToken cancellationToken)
    {
       var result= await _logService.GetLogs(request, cancellationToken);
        return result;
    }
}