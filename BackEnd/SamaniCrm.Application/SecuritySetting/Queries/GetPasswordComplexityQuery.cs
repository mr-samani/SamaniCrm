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
    public class GetPasswordComplexityQuery() : IRequest<PasswordComplexityDto>;

    public class GetPasswordComplexityQueryHandler : IRequestHandler<GetPasswordComplexityQuery, PasswordComplexityDto>
    {
        private readonly ISecuritySettingService _securitySettingService;

        public GetPasswordComplexityQueryHandler(ISecuritySettingService securitySettingService)
        {
            _securitySettingService = securitySettingService;
        }

        public async Task<PasswordComplexityDto> Handle(GetPasswordComplexityQuery request, CancellationToken cancellationToken)
        {
            var settings = await _securitySettingService.GetSettingsAsync(null, cancellationToken);
           return settings.PasswordComplexity;
        }
    }

}
