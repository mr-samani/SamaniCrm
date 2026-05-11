using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Data.Seeder;

public class TenantDataSeeder : ITenantDataSeeder
{
    public int Order => 0;

 

    public Task SeedAsync(Tenant tenant, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}

// Base class for tenant seeders
public abstract class TenantDataSeederBase<TContext> : ITenantDataSeeder
    where TContext : ApplicationDbContext
{
    protected readonly ILogger _logger;
    protected readonly ITenantDbContextFactory _contextFactory;

    public abstract int Order { get; }

    public TenantDataSeederBase(
        ILogger logger,
        ITenantDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public virtual async Task SeedAsync(Tenant tenant, CancellationToken cancellation)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(tenant.Id, cancellation);
        await SeedAsync(tenant, cancellation);
    }

    protected abstract Task SeedAsync(TContext context, ITenant tenant, CancellationToken cancellation);

   
}