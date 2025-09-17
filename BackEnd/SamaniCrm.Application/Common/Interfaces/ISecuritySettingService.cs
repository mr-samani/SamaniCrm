using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface ISecuritySettingService
    {
        Task<SecuritySettingDto> GetSettingsAsync(CancellationToken cancellationToken);
        Task<bool> SetSettingsAsync(SecuritySettingDto input, CancellationToken cancellationToken);


        Task<UserSettingDto> GetUserSettingsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> SetUserSettingsAsync(UserSettingDto input, CancellationToken cancellationToken);

    }
}
