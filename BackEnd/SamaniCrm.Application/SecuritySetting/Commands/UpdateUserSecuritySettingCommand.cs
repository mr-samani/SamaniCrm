using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Commands
{
    public class UpdateUserSecuritySettingCommand : UserSettingDto, IRequest<bool>
    {
    }


    public class UpdateUserSecuritySettingCommandHandler : IRequestHandler<UpdateUserSecuritySettingCommand, bool>
    {
        private readonly ISecuritySettingService _securitySettingService;
        private readonly ICurrentUserService _currentUser;

        public UpdateUserSecuritySettingCommandHandler(ISecuritySettingService securitySettingService, ICurrentUserService currentUser)
        {
            _securitySettingService = securitySettingService;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdateUserSecuritySettingCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            Guid userId = Guid.Parse(_currentUser.UserId);
            request.UserId = userId;
            var result = await _securitySettingService.SetUserSettingsAsync(request, cancellationToken);
            return result;
        }
    }
}
