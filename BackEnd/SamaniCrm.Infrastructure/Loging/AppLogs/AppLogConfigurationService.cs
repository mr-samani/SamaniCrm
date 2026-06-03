using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Loging.AppLogs;

public class AppLogConfigurationService : IAppLogConfigurationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public AppLogConfigurationService(ICacheService cache, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    public async Task<TenantAppLogSettingDto?> GetSettingAsync(Guid? tenantId, CancellationToken cancellation)
    {
        try
        {
            var cacheKey = $"LogSetting_{tenantId}";
            TenantAppLogSettingDto? setting = await _cache.GetAsync<TenantAppLogSettingDto>(cacheKey);
            if (setting != null)
                return setting;

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                setting = await dbContext.TenantLogSettings
                      .Select(s => new TenantAppLogSettingDto()
                      {
                          TenantId = s.TenantId,
                          IsEnabled = s.IsEnabled,
                          EnabledLevels = s.EnabledLevels,
                          EnabledSinks = s.EnabledSinks,
                          RetentionDays = s.RetentionDays,
                          CustomSettings = s.CustomSettings
                      })
                      .AsNoTracking()
                      .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellation);

                if (setting == null)
                {
                    // تنظیمات پیش‌فرض
                    setting = new TenantAppLogSettingDto
                    {
                        TenantId = tenantId,
                        IsEnabled = true,
                        EnabledLevels = LogLevelMask.All,
                        EnabledSinks = LogSinkMask.Database,
                        RetentionDays = 30
                    };
                }

                await _cache.SetAsync(cacheKey, setting, CacheDuration);
                return setting;
            }

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UpdateSettingAsync(TenantAppLogSettingDto setting, CancellationToken cancellation)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            var existing = await dbContext.TenantLogSettings
            .FirstOrDefaultAsync(s => s.TenantId == setting.TenantId, cancellation);

            if (existing == null)
            {
                var tls = new TenantAppLogSetting()
                {
                    TenantId = setting.TenantId,
                    IsEnabled = setting.IsEnabled,
                    EnabledLevels = setting.EnabledLevels,
                    EnabledSinks = setting.EnabledSinks,
                    RetentionDays = setting.RetentionDays,
                    CustomSettings = setting.CustomSettings
                };

                dbContext.TenantLogSettings.Add(tls);
            }
            else
            {
                existing.EnabledLevels = setting.EnabledLevels;
                existing.EnabledSinks = setting.EnabledSinks;
                existing.RetentionDays = setting.RetentionDays;
                existing.IsEnabled = setting.IsEnabled;
                existing.CustomSettings = setting.CustomSettings;
                existing.ModifiedAt = DateTime.UtcNow;
            }

            var result = await dbContext.SaveChangesAsync(cancellation);

            // پاک کردن Cache
            await _cache.RemoveAsync($"LogSetting_{setting.TenantId}");
            return result > 0;
        }
    }

    public async Task<bool> ShouldLogAsync(Guid? tenantId, LogLevel level, CancellationToken cancellation)
    {
        var setting = await GetSettingAsync(tenantId, cancellation);

        if (setting == null || !setting.IsEnabled)
            return false;

        var levelMask = GetLevelMask(level);
        return setting.EnabledLevels.HasFlag(levelMask);
    }

    public async Task<LogSinkMask> GetEnabledSinksAsync(Guid? tenantId, CancellationToken cancellation)
    {
        var setting = await GetSettingAsync(tenantId, cancellation);
        return setting?.EnabledSinks ?? LogSinkMask.Database;
    }

    private static LogLevelMask GetLevelMask(LogLevel level) => level switch
    {
        LogLevel.Trace => LogLevelMask.Trace,
        LogLevel.Debug => LogLevelMask.Debug,
        LogLevel.Information => LogLevelMask.Information,
        LogLevel.Warning => LogLevelMask.Warning,
        LogLevel.Error => LogLevelMask.Error,
        LogLevel.Critical => LogLevelMask.Critical,
        _ => LogLevelMask.None
    };
}
