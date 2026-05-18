using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Logging.Queries;

namespace SamaniCrm.Application.Features.Logging.Interfaces;

public interface ILogService
{
    void LogTrace(string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception? exception, string message, params object[] args);
    void LogCritical(Exception? exception, string message, params object[] args);

    // لاگ با اطلاعات کامل
    void Log(LogLevel level, string message,
             Exception? exception = null, LogContextDto? context = null, params object[] args);


    Task<PaginatedResult<LogEntryDto>> GetLogs(GetLogsQuery filter, CancellationToken cancellation);
    Task<LogEntryDto> GetLogDetails(GetLogDetailsQuery filter, CancellationToken cancellation);

    Task<LogStatsDto> GetStats(DateTime fromDate, DateTime toDate, Guid? tenantId, CancellationToken cancellation);
    Task<CleanupLogResultDto> ManualCleanupLog(int daysOld, Guid? tenantId, CancellationToken cancellation);




}
