using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.SecuritySetting.Commands
{
    public class UpdateSecuritySettingCommand : SecuritySettingDTO, IRequest<bool>
    {
    }


    public class UpdateSecuritySettingCommandHandler : IRequestHandler<UpdateSecuritySettingCommand, bool>
    {
        private readonly ISecuritySettingService _securitySettingService;

        public UpdateSecuritySettingCommandHandler(ISecuritySettingService securitySettingService)
        {
            _securitySettingService = securitySettingService;
        }

        public async Task<bool> Handle(UpdateSecuritySettingCommand request, CancellationToken cancellationToken)
        {
            var result = await _securitySettingService.SetSettingsAsync(request, cancellationToken);
            return result;
        }
    }
}
