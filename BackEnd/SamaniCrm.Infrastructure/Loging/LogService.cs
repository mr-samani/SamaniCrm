using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Application.Features.Logging.Queries;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Loging.Sinks;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;


namespace SamaniCrm.Infrastructure.Loging;

public class LogService : ILogService
{
    private readonly ILogConfigurationService _configService;
    private readonly IEnumerable<ILogSink> _sinks;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentUserService _currentUser;

    public LogService(
        ILogConfigurationService configService,
        IEnumerable<ILogSink> sinks,
        IHttpContextAccessor httpContextAccessor,
        IApplicationDbContext dbContext,
        ICurrentTenant currentTenant,
        ICurrentUserService currentUser)
    {
        _configService = configService;
        _sinks = sinks;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
    }

    public void LogInformation(string message, params object[] args)
        => Log(LogLevel.Information, message, null, null, args);

    public void LogWarning(string message, params object[] args)
        => Log(LogLevel.Warning, message, null, null, args);

    public void LogError(Exception? exception, string message, params object[] args)
        => Log(LogLevel.Error, message, exception, null, args);

    public void LogCritical(Exception? exception, string message, params object[] args)
        => Log(LogLevel.Critical, message, exception, null, args);

    public void LogTrace(string message, params object[] args)
        => Log(LogLevel.Trace, message, null, null, args);

    public void LogDebug(string message, params object[] args)
        => Log(LogLevel.Debug, message, null, null, args);

    public async void Log(LogLevel level,
                          string message,
                          Exception? exception = null,
                          LogContextDto? context = null,
                          params object[] args)
    {
        try
        {
            if (context == null)
            {
                context = new LogContextDto();
            }
            // تنظیم TenantId
            context.TenantId = _currentTenant.TenantId;

            // پر کردن اطلاعات HTTP
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                context.IpAddress ??= httpContext.Connection.RemoteIpAddress?.ToString();
                context.CorrelationId ??= httpContext.TraceIdentifier;
                context.HttpMethod ??= httpContext.Request.Method;
                context.RequestPath ??= httpContext.Request.Path;

                // کاربر
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    context.UserId ??= _currentUser.UserId;
                    context.UserName ??= _currentUser.UserName;
                }
            }

            // بررسی آیا باید لاگ شود
            if (!await _configService.ShouldLogAsync(context.TenantId, level, CancellationToken.None))
                return;

            // ساخت LogEntry
            var logEntry = new LogEntry
            {
                TenantId = context.TenantId,
                Level = level,
                Message = string.Format(message, args),
                ExceptionDetails = exception?.ToString(),
                UserId = context.UserId,
                UserName = context.UserName,
                IpAddress = context.IpAddress,
                CorrelationId = context.CorrelationId,
                Source = context.Source,
                ActionName = context.ActionName,
                ControllerName = context.ControllerName,
                HttpMethod = context.HttpMethod,
                RequestPath = context.RequestPath,
                ExtraData = context.ExtraData,
                Timestamp = DateTime.UtcNow
            };

            // ارسال به تمام Sink های فعال
            var enabledSinks = await _configService.GetEnabledSinksAsync(context.TenantId, CancellationToken.None);

            foreach (var sink in _sinks)
            {
                if (enabledSinks.HasFlag(GetSinkMask(sink)))
                {
                    await sink.WriteAsync(logEntry);
                }
            }
        }
        catch (Exception ex)
        {
            // Fallback به Console در صورت خطا
            Console.Error.WriteLine($"[LOG ERROR] {ex.Message}");
        }
    }

    private static LogSinkMask GetSinkMask(ILogSink sink) => sink switch
    {
        FileLogSink => LogSinkMask.File,
        DatabaseLogSink => LogSinkMask.Database,
        TelegramLogSink => LogSinkMask.Telegram,
        ExternalApiLogSink => LogSinkMask.ExternalApi,
        _ => LogSinkMask.None
    };




    public async Task<PaginatedResult<LogEntryDto>> GetLogs(GetLogsQuery filter, CancellationToken cancellation)
    {
        var query = _dbContext.LogEntries
            .AsNoTracking()
            .Where(l => l.TenantId == filter.TenantId);

        // فیلترها
        if (filter.Level.HasValue)
            query = query.Where(l => l.Level == filter.Level.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(l => l.Timestamp >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(l => l.Timestamp <= filter.ToDate.Value);

        if (filter.UserId.HasValue)
            query = query.Where(l => l.UserId == filter.UserId.Value);

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(l => l.Message.Contains(filter.Search) ||
                                     l.ExceptionDetails != null &&
                                     l.ExceptionDetails.Contains(filter.Search));



        var total = await query.CountAsync(cancellation);

        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            var sortString = $"{filter.SortBy} {filter.SortDirection}";
            query = query.OrderBy(sortString);
        }

        var items = await query
                            .Select(l => new LogEntryDto
                            {
                                Id = l.Id,
                                Level = l.Level,
                                Message = l.Message,
                                ExceptionDetails = l.ExceptionDetails,
                                Source = l.Source,
                                UserId = l.UserId,
                                UserName = l.UserName,
                                IpAddress = l.IpAddress,
                                Timestamp = l.Timestamp,
                                CorrelationId = l.CorrelationId
                            })
                          .Skip((filter.PageNumber - 1) * filter.PageSize)
                          .Take(filter.PageSize)
                          .ToListAsync(cancellation);
        return new PaginatedResult<LogEntryDto>(items, total, filter.PageNumber, filter.PageSize);

    }

    public async Task<LogEntryDto> GetLogDetails(GetLogDetailsQuery request, CancellationToken cancellation)
    {
        var result = await _dbContext.LogEntries
                .AsNoTracking()
                .Where(l => l.TenantId == request.tenantId && l.Id == request.logId)
                .Select(log => new LogEntryDto
                {
                    Id = log.Id,
                    Level = log.Level,
                    Message = log.Message,
                    ExceptionDetails = log.ExceptionDetails,
                    Source = log.Source,
                    ControllerName = log.ControllerName,
                    ActionName = log.ActionName,
                    HttpMethod = log.HttpMethod,
                    RequestPath = log.RequestPath,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    IpAddress = log.IpAddress,
                    Timestamp = log.Timestamp,
                    CorrelationId = log.CorrelationId,
                    ExtraData = log.ExtraData
                }).FirstOrDefaultAsync(cancellation);
        ;

        if (result == null)
            throw new NotFoundException("Log not found!");

        return result;
    }


    public async Task<LogStatsDto> GetStats(DateTime fromDate, DateTime toDate, Guid? tenantId, CancellationToken cancellation)
    {
        var stats = await _dbContext.LogEntries
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId && l.Timestamp >= fromDate && l.Timestamp <= toDate)
            .GroupBy(l => l.Level)
            .Select(g => new { Level = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalCount = await _dbContext.LogEntries
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId && l.Timestamp >= fromDate && l.Timestamp <= toDate)
            .CountAsync();

        return new LogStatsDto
        {
            TotalCount = totalCount,
            FromDate = fromDate,
            ToDate = toDate,
            LevelCounts = stats.ToDictionary(s => s.Level.ToString(), s => s.Count)
        };
    }

    public async Task<CleanupLogResultDto> ManualCleanupLog(int daysOld, Guid? tenantId, CancellationToken cancellation)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

        var deletedCount = await _dbContext.LogEntries
            .Where(l => l.TenantId == tenantId && l.Timestamp < cutoffDate)
            .ExecuteDeleteAsync();

        return new CleanupLogResultDto
        {
            DeletedCount = deletedCount,
            CutoffDate = cutoffDate
        };
    }



}
