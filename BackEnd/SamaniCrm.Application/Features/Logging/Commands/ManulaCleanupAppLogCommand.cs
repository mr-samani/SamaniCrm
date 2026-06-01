using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Commands;

public class ManulaCleanupAppLogCommand : IRequest<CleanupLogResultDto>
{
    public int DaysOld { get; set; } = 30;
    public Guid? TenantId { get; set; }
}


public class ManulaCleanupAppLogCommandHandler : IRequestHandler<ManulaCleanupAppLogCommand, CleanupLogResultDto>
{
    private readonly IAppLogService _logService;

    public ManulaCleanupAppLogCommandHandler(IAppLogService logService)
    {
        _logService = logService;
    }

    public async Task<CleanupLogResultDto> Handle(ManulaCleanupAppLogCommand request, CancellationToken cancellationToken)
    {
        var result = await _logService.ManualCleanupLog(request.DaysOld, request.TenantId, cancellationToken);
        return result;
    }
}