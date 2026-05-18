using MediatR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetLogsQuery : PaginationRequest, IRequest<PaginatedResult<LogEntryDto>>
{
    public Guid? TenantId { get; set; }
    public LogLevel? Level { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? UserId { get; set; }
    public string? Search { get; set; }
}

public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, PaginatedResult<LogEntryDto>>
{
    private readonly ILogService _logService;

    public GetLogsQueryHandler(ILogService logService)
    {
        _logService = logService;
    }

    public async Task<PaginatedResult<LogEntryDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
       var result= await _logService.GetLogs(request, cancellationToken);
        return result;
    }
}