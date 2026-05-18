using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Loging.Sinks;

public class DatabaseLogSink : ILogSink
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<DatabaseLogSink> _logger;

    public string Name => "Database";

    public DatabaseLogSink(IApplicationDbContext context, ILogger<DatabaseLogSink> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task WriteAsync(LogEntry entry)
    {
        try
        {
            _dbContext.LogEntries.Add(entry);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write log to database");
        }
    }
}
