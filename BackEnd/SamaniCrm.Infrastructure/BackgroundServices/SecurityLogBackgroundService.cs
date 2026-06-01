using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Loging.SecurityLogs;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SamaniCrm.Infrastructure.BackgroundServices;

public sealed class SecurityLogBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SecurityLogQueue _queue;
    private readonly ILogger<SecurityLogBackgroundService> _logger;

    private const int BatchSize = 1000;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // مثلا هر ۱ دقیقه چک کن

    public SecurityLogBackgroundService(
        IServiceScopeFactory scopeFactory,
        SecurityLogQueue queue,
        ILogger<SecurityLogBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buffer = new List<SecurityLogDto>(BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                buffer.Clear();

                var hasItem = await _queue.Reader.WaitToReadAsync(stoppingToken);

                if (!hasItem)
                    continue;

                while (_queue.Reader.TryRead(out var item))
                {
                    buffer.Add(item);

                    if (buffer.Count >= BatchSize)
                        break;
                }

                var timeout = Task.Delay(_interval, stoppingToken);

                while (buffer.Count < BatchSize)
                {
                    var readTask = _queue.Reader.WaitToReadAsync(stoppingToken).AsTask();

                    var completed = await Task.WhenAny(readTask, timeout);

                    if (completed == timeout)
                        break;

                    while (_queue.Reader.TryRead(out var item))
                    {
                        buffer.Add(item);

                        if (buffer.Count >= BatchSize)
                            break;
                    }
                }

                if (buffer.Count > 0)
                {
                    await SaveBatch(buffer, stoppingToken);
                    _logger.LogInformation($"Security log inserted: {buffer.Count}", buffer.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while persisting security logs");
            }
        }
    }

    private async Task SaveBatch(List<SecurityLogDto> logs, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var connectionString = scope.ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

        await using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync(ct);

        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null);

        bulkCopy.BatchSize = logs.Count;
        bulkCopy.DestinationTableName = "logs.SecurityLogEntries";

        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Id), "Id");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.CorrelationId), "CorrelationId");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.TenantId), "TenantId");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.EventType), "EventType");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Severity), "Severity");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.UserId), "UserId");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Username), "Username");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.IpAddress), "IpAddress");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.UserAgent), "UserAgent");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Action), "Action");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Resource), "Resource");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.StatusCode), "StatusCode");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.IsSuccessful), "IsSuccessful");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.Message), "Message");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.CreatedAt), "CreatedAt");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.CreatedBy), "CreatedBy");
        bulkCopy.ColumnMappings.Add(nameof(SecurityLogEntry.IntegrityHash), "IntegrityHash");

        using var table = CreateDataTable(logs);

        await bulkCopy.WriteToServerAsync(table, ct);
    }

    private static DataTable CreateDataTable(IEnumerable<SecurityLogDto> logs)
    {
        var table = new DataTable();

        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("TenantId", typeof(Guid));
        table.Columns.Add("EventType", typeof(int));
        table.Columns.Add("Severity", typeof(int));
        table.Columns.Add("UserId", typeof(Guid));
        table.Columns.Add("Username", typeof(string));
        table.Columns.Add("IpAddress", typeof(string));
        table.Columns.Add("UserAgent", typeof(string));
        table.Columns.Add("Action", typeof(string));
        table.Columns.Add("Resource", typeof(string));
        table.Columns.Add("StatusCode", typeof(int));
        table.Columns.Add("IsSuccessful", typeof(bool));
        table.Columns.Add("Message", typeof(string));
        table.Columns.Add("CorrelationId", typeof(string));
        table.Columns.Add("CreatedAt", typeof(DateTime));
        table.Columns.Add("CreatedBy", typeof(string));
        table.Columns.Add("IntegrityHash", typeof(string));

        foreach (var item in logs)
        {

            var entity = new SecurityLogEntry
            {
                Id = item.Id,
                CorrelationId = item.CorrelationId,
                TenantId = item.TenantId,
                EventType = item.EventType,
                Severity = item.Severity,
                UserId = item.UserId,
                Username = item.Username,
                IpAddress = item.IpAddress ?? string.Empty,
                UserAgent = item.UserAgent,
                Action = item.Action ?? string.Empty,
                Resource = item.Resource,
                StatusCode = item.StatusCode,
                IsSuccessful = item.IsSuccessful,
                Message = item.Message,
                CreatedAt = item.CreatedAt,
                CreatedBy = item.CreatedBy ?? "System",
                IntegrityHash = string.Empty
            };

            entity.CalculateIntegrityHash();

            var row = table.NewRow();
            row["Id"] = entity.Id;
            row["CorrelationId"] = entity.CorrelationId;
            row["TenantId"] = entity.TenantId.HasValue ? entity.TenantId.Value : DBNull.Value;
            row["EventType"] = (int)entity.EventType;
            row["Severity"] = (int)entity.Severity;
            row["UserId"] = entity.UserId.HasValue ? entity.UserId.Value : DBNull.Value;
            row["Username"] = string.IsNullOrEmpty(entity.Username) ? DBNull.Value : entity.Username;
            row["IpAddress"] = entity.IpAddress;
            row["UserAgent"] = string.IsNullOrEmpty(entity.UserAgent) ? DBNull.Value : entity.UserAgent;
            row["Action"] = entity.Action;
            row["Resource"] = string.IsNullOrEmpty(entity.Resource) ? DBNull.Value : entity.Resource;
            row["StatusCode"] = entity.StatusCode.HasValue ? entity.StatusCode.Value : DBNull.Value;
            row["IsSuccessful"] = entity.IsSuccessful;
            row["Message"] = string.IsNullOrEmpty(entity.Message) ? DBNull.Value : entity.Message;
            row["CreatedAt"] = entity.CreatedAt;
            row["CreatedBy"] = entity.CreatedBy;
            row["IntegrityHash"] = entity.IntegrityHash;

            table.Rows.Add(row);
        }

        return table;
    }
}