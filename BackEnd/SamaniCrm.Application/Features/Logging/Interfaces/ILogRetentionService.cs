using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Logging.Interfaces;

// مدیریت Retention (نگهداری لاگ)
public interface ILogRetentionService
{
    Task<int> CleanupOldLogsAsync(Guid tenantId, CancellationToken ct = default);
    Task<int> CleanupAllTenantsAsync(CancellationToken ct = default);
}