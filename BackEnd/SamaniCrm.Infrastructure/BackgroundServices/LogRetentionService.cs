using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.BackgroundServices;

public class LogRetentionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogRetentionService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    public LogRetentionService(IServiceScopeFactory scopeFactory,
                               ILogger<LogRetentionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Log Retention Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupLogsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during log cleanup");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CleanupLogsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var settings = await context.TenantLogSettings
            .Where(s => s.IsEnabled && s.RetentionDays.HasValue)
            .ToListAsync(ct);

        foreach (var setting in settings)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-setting.RetentionDays!.Value);

            var deletedCount = await context.LogEntries
                .Where(l => l.TenantId == setting.TenantId && l.Timestamp < cutoffDate)
                .ExecuteDeleteAsync(ct);

            if (deletedCount > 0)
            {
                _logger.LogInformation(
                    "Cleaned up {Count} old logs for tenant {TenantId} (older than {Days} days)",
                    deletedCount, setting.TenantId, setting.RetentionDays);
            }
        }
    }
}