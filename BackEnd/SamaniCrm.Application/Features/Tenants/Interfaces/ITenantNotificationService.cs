using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ITenantNotificationService
{
    Task SendProgressAsync(string tenantSlug, string message, TenantProvisionStepsEnum step);
    Task SendCompletionAsync(string tenantSlug, string message, TenantProvisionStepsEnum step);
    Task SendErrorAsync(string tenantSlug, string errorMessage, TenantProvisionStepsEnum step);
}
