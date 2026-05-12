using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Infrastructure.Services.TenantService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Data;

public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITenantDatabaseService _databaseService;
    private readonly ITenantResolver _tenantResolver;
    private readonly ILogger<TenantDbContextFactory> _logger;
    private readonly ConcurrentDictionary<Guid, string> _connectionStringCache;

    public TenantDbContextFactory(
        ITenantDatabaseService databaseService,
        ITenantResolver tenantResolver,
        ILogger<TenantDbContextFactory> logger,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUser)
    {
        _databaseService = databaseService;
        _tenantResolver = tenantResolver;
        _logger = logger;
        _connectionStringCache = new ConcurrentDictionary<Guid, string>();
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public ApplicationDbContext CreateDbContext(Guid tenantId)
    {
        var connectionString = GetConnectionString(tenantId);
        return CreateDbContext(connectionString);
    }

    public ApplicationDbContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        });

        // Add interceptors
        optionsBuilder.AddInterceptors(
            new TenantQueryInterceptor()
            // TODO:
            // , new TenantCommandInterceptor()
            );

        return new ApplicationDbContext(optionsBuilder.Options, _currentUser);
    }

    public async Task<ApplicationDbContext> CreateDbContextAsync(Guid tenantId, CancellationToken cancellation)
    {
        var connectionString = await GetConnectionStringAsync(tenantId, cancellation);
        return CreateDbContext(connectionString);
    }

    public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellation);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellation);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection for tenant");
            return false;
        }
    }

    private string GetConnectionString(Guid tenantId)
    {
        // Check cache first
        if (_connectionStringCache.TryGetValue(tenantId, out var cached))
        {
            return cached;
        }

        // Get from database
        var tenant = _dbContext.TenantDatabaseConnections
            .FirstOrDefault(c => c.TenantId == tenantId && c.IsActive);

        if (tenant == null)
        {
            throw new InvalidOperationException($"No database connection found for tenant {tenantId}");
        }

        var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
        _connectionStringCache.TryAdd(tenantId, connectionString);

        return connectionString;
    }

    private async Task<string> GetConnectionStringAsync(Guid tenantId, CancellationToken cancellation)
    {
        // Check cache first
        if (_connectionStringCache.TryGetValue(tenantId, out var cached))
        {
            return cached;
        }

        // Get from database
        var tenant = await _dbContext.TenantDatabaseConnections
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.IsActive, cancellation);

        if (tenant == null)
        {
            throw new InvalidOperationException($"No database connection found for tenant {tenantId}");
        }

        var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
        _connectionStringCache.TryAdd(tenantId, connectionString);

        return connectionString;
    }

    public void ClearCache(Guid? tenantId = null)
    {
        if (tenantId.HasValue)
        {
            _connectionStringCache.TryRemove(tenantId.Value, out _);
        }
        else
        {
            _connectionStringCache.Clear();
        }
    }
}

// Query Interceptor for multi-tenant filtering
public class TenantQueryInterceptor : IInterceptor
{
    public InterceptionResult ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult result)
    {
        // Add tenant filter hint for SQL Server
        return result;
    }

    public ValueTask<InterceptionResult> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        return new ValueTask<InterceptionResult>(result);
    }
}

// Command Interceptor for logging and audit
public class TenantCommandInterceptor : IInterceptor
{
    private readonly ILogger<TenantCommandInterceptor> _logger;

    public TenantCommandInterceptor(ILogger<TenantCommandInterceptor> logger)
    {
        _logger = logger;
    }

    public InterceptionResult NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult result)
    {
        LogCommand(command.CommandText, eventData);
        return result;
    }

    public async ValueTask<InterceptionResult> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command.CommandText, eventData);
        return result;
    }

    private void LogCommand(string commandText, CommandEventData? eventData)
    {
        if (commandText.Contains("INSERT", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("UPDATE", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Executing SQL: {Command}", commandText);
        }
    }
}