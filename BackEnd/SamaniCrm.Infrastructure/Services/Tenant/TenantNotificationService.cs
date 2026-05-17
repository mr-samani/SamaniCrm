using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Infrastructure.Hubs;

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

    public async Task SendProgressAsync(string tenantSlug, TenantProvisionStepsEnum step, string? message)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningStepStatus.InProgress,
            CurrentStep = step,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnProgress(notification);

        _logger.LogDebug("Sent progress notification for {TenantSlug}: Step {Step}", tenantSlug, step);
    }

    public async Task SendCompletionAsync(string tenantSlug, TenantProvisionStepsEnum step, string? message)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningStepStatus.Completed,
            CurrentStep = step,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnCompleted(notification);

        _logger.LogInformation("Sent completion notification for {TenantSlug}", tenantSlug);
    }

    public async Task SendErrorAsync(string tenantSlug, TenantProvisionStepsEnum step, string? errorMessage)
    {
        var notification = new ProvisioningNotification
        {
            TenantSlug = tenantSlug,
            Status = ProvisioningStepStatus.Failed,
            CurrentStep = step,
            Message = errorMessage,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients
            .Group($"tenant-{tenantSlug}")
            .OnError(notification);

        _logger.LogError("Sent error notification for {TenantSlug}: {Error}", tenantSlug, errorMessage);
    }
}
