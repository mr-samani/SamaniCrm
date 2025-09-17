using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Queries
{
    public record GetUserSecuritySettingsQuery : IRequest<UserSettingDto>;

    public class GetUserSecuritySettingsQueryHandler : IRequestHandler<GetUserSecuritySettingsQuery, UserSettingDto>
    {
        private readonly ISecuritySettingService _securitySettingService;
        private readonly ICurrentUserService _currentUser;

        public GetUserSecuritySettingsQueryHandler(ISecuritySettingService securitySettingService, ICurrentUserService currentUser)
        {
            _securitySettingService = securitySettingService;
            _currentUser = currentUser;
        }

        public async Task<UserSettingDto> Handle(GetUserSecuritySettingsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_currentUser.UserId))
            {
                throw new AccessDeniedException();
            }
            Guid userId = Guid.Parse(_currentUser.UserId);
            return await _securitySettingService.GetUserSettingsAsync(userId, cancellationToken);
        }
    }

}
