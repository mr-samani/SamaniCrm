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
        Task<SecuritySettingDTO> GetSettingsAsync(CancellationToken cancellationToken);
        Task<bool> SetSettingsAsync(SecuritySettingDTO input, CancellationToken cancellationToken);
    }
}
