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
    public class GetPasswordComplexityQuery() : IRequest<PasswordComplexityDTO>;

    public class GetPasswordComplexityQueryHandler : IRequestHandler<GetPasswordComplexityQuery, PasswordComplexityDTO>
    {
        private readonly ISecuritySettingService _securitySettingService;

        public GetPasswordComplexityQueryHandler(ISecuritySettingService securitySettingService)
        {
            _securitySettingService = securitySettingService;
        }

        public async Task<PasswordComplexityDTO> Handle(GetPasswordComplexityQuery request, CancellationToken cancellationToken)
        {
            var settings = await _securitySettingService.GetSettingsAsync();
           return settings.PasswordComplexity;
        }
    }

}
