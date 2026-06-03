using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;


namespace SamaniCrm.Application.Common.Interfaces;

public interface IMasterDbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections { get; set; }
    public DbSet<TenantCategory> TenantCategories { get; set; }
    public DbSet<TenantProvisioningStep> TenantProvisioningSteps { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }

    public DbSet<Language> Languages { get; set; }
    public DbSet<Localization> Localizations { get; set; }


    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<ExternalProvider> ExternalProviders { get; set; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
