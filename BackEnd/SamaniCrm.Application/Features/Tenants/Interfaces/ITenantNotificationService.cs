using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ITenantNotificationService
{
    Task SendProgressAsync(string tenantSlug, TenantProvisionStepsEnum step, string? message);
    Task SendCompletionAsync(string tenantSlug, TenantProvisionStepsEnum step, string? message);
    Task SendErrorAsync(string tenantSlug, TenantProvisionStepsEnum step, string? errorMessage);
}
