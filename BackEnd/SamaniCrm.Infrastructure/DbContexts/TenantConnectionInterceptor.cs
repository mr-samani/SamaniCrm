using Microsoft.EntityFrameworkCore.Diagnostics;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure.TenantManager;
using System.Data;
using System.Data.Common;


public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenant _currentTenant;

    public TenantConnectionInterceptor(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        ReplaceConnectionString(connection);

        return result;
    }

    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        ReplaceConnectionString(connection);

        return ValueTask.FromResult(result);
    }

    private void ReplaceConnectionString(DbConnection connection)
    {
        if (connection.State != ConnectionState.Closed)
            return;

        var connectionString = _currentTenant.GetCurrentConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        if (connection.ConnectionString == connectionString)
            return;

        Console.WriteLine($"CS+++== {connectionString}");
        connection.ConnectionString = connectionString;
    }
}