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
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Loging.Sinks;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


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

            var msg = FormatNamed(message, args);

            // ساخت LogEntry
            var logEntry = new LogEntry
            {
                TenantId = context.TenantId,
                Level = level,
                Message = msg,
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
                Timestamp = DateTime.UtcNow,
                ExtraData = context.ExtraData
            };


            // ارسال به تمام Sink های فعال
            var enabledSinks = await _configService.GetEnabledSinksAsync(context.TenantId, CancellationToken.None);

            foreach (var sink in _sinks)
            {
                if (enabledSinks.HasFlag(GetSinkMask(sink)))
                {
                    try
                    {
                        await sink.WriteAsync(logEntry);
                    }
                    catch (Exception ex)
                    {
                        // اگر در ثبت یکی از منبع های لاگ خطا رخ داد - بیخیال بشه و بره سراغ لاگ روی منبع بعدی اگر منبعی داشت
                        // مثلا ارسال لاگ به تلگرام اگر خطا داد متوقف نشود بره سراغ ارسال لاگ بعدی
                    }
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
        BaleLogSink => LogSinkMask.Bale,
        ExternalApiLogSink => LogSinkMask.ExternalApi,
        _ => LogSinkMask.None
    };




    public async Task<PaginatedResult<LogEntryDto>> GetLogs(GetLogsQuery filter, CancellationToken cancellation)
    {
        // ابتدا فیلترها را اعمال می‌کنیم
        var baseQuery = _dbContext.LogEntries
            .AsNoTracking()
            .Where(l => l.TenantId == filter.TenantId);

        // فیلتر سطح با Flags
        if (filter.Levels != null && filter.Levels.Any())
            baseQuery = baseQuery.Where(l => filter.Levels.Contains(l.Level));

        if (filter.FromDate.HasValue)
            baseQuery = baseQuery.Where(l => l.Timestamp >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            baseQuery = baseQuery.Where(l => l.Timestamp <= filter.ToDate.Value);
        if (filter.UserId.HasValue)
            baseQuery = baseQuery.Where(l => l.UserId == filter.UserId.Value);
        if (!string.IsNullOrEmpty(filter.Search))
            baseQuery = baseQuery.Where(l => l.Message.Contains(filter.Search) ||
                                             (l.ExceptionDetails != null &&
                                              l.ExceptionDetails.Contains(filter.Search)));



        // گروه‌بندی بر اساس CorrelationId و محاسبه Duration
        var groupedQuery = baseQuery
            .GroupBy(l => l.CorrelationId)
            .Select(g => new
            {
                CorrelationId = g.Key,
                MinTimestamp = g.Min(x => x.Timestamp),
                MaxTimestamp = g.Max(x => x.Timestamp),
                // فقط فیلدهایی که برای ساخت DTO لازم است
                StartId = g.OrderBy(x => x.Timestamp).Select(x => x.Id).First(),
                StartLevel = g.OrderBy(x => x.Timestamp).Select(x => x.Level).First(),
                StartMessage = g.OrderBy(x => x.Timestamp).Select(x => x.Message).First(),
                StartControllerName = g.OrderBy(x => x.Timestamp).Select(x => x.ControllerName).First(),
                StartActionName = g.OrderBy(x => x.Timestamp).Select(x => x.ActionName).First(),
                StartHttpMethod = g.OrderBy(x => x.Timestamp).Select(x => x.HttpMethod).First(),
                StartRequestPath = g.OrderBy(x => x.Timestamp).Select(x => x.RequestPath).First(),
                StartSource = g.OrderBy(x => x.Timestamp).Select(x => x.Source).First(),
                StartUserId = g.OrderBy(x => x.Timestamp).Select(x => x.UserId).First(),
                StartUserName = g.OrderBy(x => x.Timestamp).Select(x => x.UserName).First(),
                StartIpAddress = g.OrderBy(x => x.Timestamp).Select(x => x.IpAddress).First(),
                StartTimestamp = g.OrderBy(x => x.Timestamp).Select(x => x.Timestamp).First(),
            });

        // شمارش کل قبل از صفحه‌بندی
        var total = await groupedQuery.CountAsync(cancellation);


        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            var sortString = $"Start{filter.SortBy} {filter.SortDirection}";
            groupedQuery = groupedQuery.OrderBy(sortString);
        }


        // صفحه‌بندی
        var rawItems = await groupedQuery
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellation);
        // مرحله ۲: محاسبه Duration و ساخت DTO در حافظه
        var items = rawItems
            .Select(x => new LogEntryDto
            {
                Id = x.StartId,
                Level = x.StartLevel,
                Message = x.StartMessage,
                ControllerName = x.StartControllerName,
                ActionName = x.StartActionName,
                HttpMethod = x.StartHttpMethod,
                RequestPath = x.StartRequestPath,
                Source = x.StartSource,
                UserId = x.StartUserId,
                UserName = x.StartUserName,
                IpAddress = x.StartIpAddress,
                Timestamp = x.StartTimestamp,
                CorrelationId = x.CorrelationId,
                Duration = (long?)(x.MaxTimestamp - x.MinTimestamp).TotalMilliseconds
            })
            .ToList();


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

    public static string FormatNamed(string template, params object[] args)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        var result = template;

        // پیدا کردن تمام placeholder ها: {Name}
        var matches = Regex.Matches(template, @"\{(\w+)\}");

        for (int i = 0; i < args.Length && i < matches.Count; i++)
        {
            var placeholder = matches[i].Value;
            var value = args[i]?.ToString() ?? "";
            value = value.Replace("{", "{{").Replace("}", "}}");

            result = result.Replace(placeholder, value);
        }

        return result;
    }

}




