using Microsoft.Extensions.Configuration;
using SamaniCrm.Domain.Entities;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Loging.Sinks;

public class FileLogSink : ILogSink
{
    private readonly string _basePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private StreamWriter? _writer;
    private DateTime _currentDate;

    public string Name => "File";

    public FileLogSink(IConfiguration configuration)
    {
        _basePath = configuration["Logging:File:Path"] ?? "logs";
    }

    public Task InitializeAsync()
    {
        EnsureNewFile();
        return Task.CompletedTask;
    }

    public async Task WriteAsync(LogEntry entry)
    {
        await _semaphore.WaitAsync();
        try
        {
            EnsureNewFile();

            var logLine = JsonSerializer.Serialize(new
            {
                entry.Timestamp,
                Level = entry.Level.ToString(),
                entry.Message,
                entry.ExceptionDetails,
                entry.CorrelationId,
                entry.UserId,
                entry.Source,
                entry.ExtraData
            });

            await _writer!.WriteLineAsync(logLine);
            await _writer.FlushAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void EnsureNewFile()
    {
        if (_currentDate != DateTime.UtcNow.Date || _writer == null)
        {
            _writer?.Dispose();

            var directory = Path.Combine(_basePath, DateTime.UtcNow.ToString("yyyy/MM/dd"));
            Directory.CreateDirectory(directory);

            var fileName = Path.Combine(directory, $"app_{DateTime.UtcNow:HH}.log");
            _writer = new StreamWriter(fileName, append: true) { AutoFlush = true };
            _currentDate = DateTime.UtcNow.Date;
        }
    }
}
