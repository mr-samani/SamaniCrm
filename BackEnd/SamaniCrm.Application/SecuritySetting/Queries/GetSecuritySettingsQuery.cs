using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.SecuritySetting.Queries
{
    public record GetSecuritySettingsQuery : IRequest<SecuritySettingDto>;

    public class GetSecuritySettingsQueryHandler : IRequestHandler<GetSecuritySettingsQuery, SecuritySettingDto>
    {
        private readonly ISecuritySettingService _securitySettingService;
        private readonly ICurrentUserService _currentUser;

        public GetSecuritySettingsQueryHandler(ISecuritySettingService securitySettingService, ICurrentUserService currentUser)
        {
            _securitySettingService = securitySettingService;
            _currentUser = currentUser;
        }

        public async Task<SecuritySettingDto> Handle(GetSecuritySettingsQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = _currentUser.UserId != null ? Guid.Parse(_currentUser.UserId) : null;
            return await _securitySettingService.GetSettingsAsync(userId, cancellationToken);
        }
    }

}
