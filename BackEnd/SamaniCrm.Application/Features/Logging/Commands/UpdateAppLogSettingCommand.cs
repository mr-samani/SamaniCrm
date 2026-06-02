using MediatR;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Features.Logging.Commands;

public class UpdateAppLogSettingCommand : TenantAppLogSettingDto, IRequest<bool>
{
}



public class UpdateAppLogSettingCommandHandler : IRequestHandler<UpdateAppLogSettingCommand, bool>
{
    private readonly IAppLogConfigurationService _configService;

    public UpdateAppLogSettingCommandHandler(IAppLogConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task<bool> Handle(UpdateAppLogSettingCommand request, CancellationToken cancellation)
    {
        var result = await _configService.UpdateSettingAsync(request, cancellation);

        return result;
    }
}