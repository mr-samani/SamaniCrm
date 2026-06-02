using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Queries;

public class GetLastLoginInfoQuery : IRequest<List<LastLoginDto>>
{
    public int Take { get; set; } = 50;
}

public class GetLastLoginInfoQueryHandler : IRequestHandler<GetLastLoginInfoQuery, List<LastLoginDto>>
{
    private readonly ISecurityLogService _securityLogService;

    public GetLastLoginInfoQueryHandler(ISecurityLogService securityLogService)
    {
        _securityLogService = securityLogService;
    }

    public async Task<List<LastLoginDto>> Handle(GetLastLoginInfoQuery request, CancellationToken cancellationToken)
    {
        var result = await _securityLogService.GetLastLoginInfo(request, cancellationToken);
        return result;
    }
}
