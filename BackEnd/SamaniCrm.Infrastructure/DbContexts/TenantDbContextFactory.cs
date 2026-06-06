using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Infrastructure.DbContexts;


namespace SamaniCrm.Infrastructure.DbContexts;

public interface ITenantDbContextFactory
{
    TenantDbContext Create(string connectionString);
}
public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly IServiceProvider _sp;

    public TenantDbContextFactory(IServiceProvider sp)
    {
        _sp = sp;
    }

    public TenantDbContext Create(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<TenantDbContext>();

        builder.UseSqlServer(connectionString);

        return ActivatorUtilities.CreateInstance<TenantDbContext>(
            _sp,
            builder.Options);
    }
}
