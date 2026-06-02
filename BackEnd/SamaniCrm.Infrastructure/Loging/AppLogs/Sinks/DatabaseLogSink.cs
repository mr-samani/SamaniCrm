using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Loging.AppLogs.Sinks;

public class DatabaseLogSink : ILogSink
{ 
    private readonly ILogger<DatabaseLogSink> _logger;
    // به جای دیتابیس مستقیم، ScopeFactory را نگه می‌داریم
    private readonly IServiceScopeFactory _scopeFactory;
    public string Name => "Database";

    public DatabaseLogSink(ILogger<DatabaseLogSink> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task WriteAsync(AppLogEntry entry)
    {
        try
        {
            // ایجاد یک محدوده (Scope) جدید برای هر بار لاگ‌نویسی
            using (var scope = _scopeFactory.CreateScope())
            {
                // گرفتن دیتابیس از داخل scope جدید
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                dbContext.LogEntries.Add(entry);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write log to database: {message} \n details:{details}", ex.Message, ex.StackTrace);
        }
    }
}
