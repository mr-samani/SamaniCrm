using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Interfaces;

public interface ILogConfigurationService
{
    Task<TenantLogSettingDto?> GetSettingAsync(Guid? tenantId, CancellationToken cancellation);
    Task<bool> UpdateSettingAsync(TenantLogSettingDto setting, CancellationToken cancellation);
    Task<bool> ShouldLogAsync(Guid? tenantId, LogLevel level, CancellationToken cancellation);
    Task<LogSinkMask> GetEnabledSinksAsync(Guid? tenantId, CancellationToken cancellation);


}
