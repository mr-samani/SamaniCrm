using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Commands;

public class ManulaCleanupLogCommand : IRequest<CleanupLogResultDto>
{
    public int DaysOld { get; set; } = 30;
    public Guid? TenantId { get; set; }
}


public class ManulaCleanupLogCommandHandler : IRequestHandler<ManulaCleanupLogCommand, CleanupLogResultDto>
{
    private readonly ILogService _logService;

    public ManulaCleanupLogCommandHandler(ILogService logService)
    {
        _logService = logService;
    }

    public async Task<CleanupLogResultDto> Handle(ManulaCleanupLogCommand request, CancellationToken cancellationToken)
    {
        var result = await _logService.ManualCleanupLog(request.DaysOld, request.TenantId, cancellationToken);
        return result;
    }
}