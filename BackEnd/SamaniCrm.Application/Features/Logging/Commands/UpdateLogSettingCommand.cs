using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Features.Logging.Commands;

public class UpdateLogSettingCommand : TenantLogSettingDto, IRequest<bool>
{
}



public class UpdateLogSettingCommandHandler : IRequestHandler<UpdateLogSettingCommand, bool>
{
    private readonly ILogConfigurationService _configService;

    public UpdateLogSettingCommandHandler(ILogConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task<bool> Handle(UpdateLogSettingCommand request, CancellationToken cancellation)
    {
        var result = await _configService.UpdateSettingAsync(request, cancellation);

        return result;
    }
}