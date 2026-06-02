using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Interfaces;

public interface IAppLogConfigurationService
{
    Task<TenantAppLogSettingDto?> GetSettingAsync(Guid? tenantId, CancellationToken cancellation);
    Task<bool> UpdateSettingAsync(TenantAppLogSettingDto setting, CancellationToken cancellation);
    Task<bool> ShouldLogAsync(Guid? tenantId, LogLevel level, CancellationToken cancellation);
    Task<LogSinkMask> GetEnabledSinksAsync(Guid? tenantId, CancellationToken cancellation);


}
