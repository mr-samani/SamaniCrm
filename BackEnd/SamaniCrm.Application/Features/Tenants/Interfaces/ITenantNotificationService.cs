using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ITenantNotificationService
{
    Task SendProgressAsync(string tenantSlug, string message, int step,
        List<ProvisioningStep> steps, CancellationToken cancellation);
    Task SendCompletionAsync(string tenantSlug, string message, Guid tenantId,
        Guid adminUserId, CancellationToken cancellation);
    Task SendErrorAsync(string tenantSlug, string errorMessage, CancellationToken cancellation);
}
