using MediatR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetSecurityLogsQuery : PaginationRequest, IRequest<PaginatedResult<SecurityLogDto>>
{

    public string? CorrelationId { get; set; }
    public Guid? TenantId { get; set; }

    public SecurityEventType? EventType { get; set; }
    public LogLevel? Severity { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? UserId { get; set; }
    public string? Search { get; set; }
}

public class GetSecurityLogsQueryHanlder : IRequestHandler<GetSecurityLogsQuery, PaginatedResult<SecurityLogDto>>
{
    private readonly ISecurityLogService _securityLogService;

    public GetSecurityLogsQueryHanlder(ISecurityLogService securityLogService)
    {
        _securityLogService = securityLogService;
    }

    public async Task<PaginatedResult<SecurityLogDto>> Handle(GetSecurityLogsQuery request, CancellationToken cancellationToken)
    {
        var result = await _securityLogService.GetSecurityLogs(request, cancellationToken);
        return result;
    }
}