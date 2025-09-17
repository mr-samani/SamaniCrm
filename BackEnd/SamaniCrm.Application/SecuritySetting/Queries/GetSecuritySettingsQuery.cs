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

        public GetSecuritySettingsQueryHandler(ISecuritySettingService securitySettingService)
        {
            _securitySettingService = securitySettingService;
        }

        public async Task<SecuritySettingDto> Handle(GetSecuritySettingsQuery request, CancellationToken cancellationToken)
        {
            return await _securitySettingService.GetSettingsAsync(cancellationToken);
        }
    }

}
