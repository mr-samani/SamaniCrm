using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services.TenantService;


public class TenantNotificationService : ITenantNotificationService
{
    private readonly IHubContext<ProvisioningHub, IProvisioningClient> _hubContext;
    private readonly ILogger<TenantNotificationService> _logger;

    public TenantNotificationService(
        IHubContext<ProvisioningHub, IProvisioningClient> hubContext,
        ILogger<TenantNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendProgressAsync(string tenantSlug, string message, int step,
        List<ProvisioningStep> steps, CancellationToken cancellation)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.InProgress,
            CurrentStep = step,
            TotalSteps = steps.Count,
            Message = message,
            Steps = steps,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnProgress(notification);

        _logger.LogDebug("Sent progress notification for {TenantSlug}: Step {Step}/{Total}",
            tenantSlug, step, steps.Count);
    }

    public async Task SendCompletionAsync(string tenantSlug, string message,
        Guid tenantId, Guid adminUserId, CancellationToken cancellation)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.Completed,
            Message = message,
            TenantId = tenantId,
            AdminUserId = adminUserId,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnCompleted(notification);

        _logger.LogInformation("Sent completion notification for {TenantSlug}", tenantSlug);
    }

    public async Task SendErrorAsync(string tenantSlug, string errorMessage,
        CancellationToken cancellation)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningNotificationStatus.Failed,
            Message = errorMessage,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnError(notification);

        _logger.LogError("Sent error notification for {TenantSlug}: {Error}", tenantSlug, errorMessage);
    }
}
