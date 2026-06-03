using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Services.TenantService;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts;

public class MasterDbContext : DbContext, IMasterDbContext
{

    private readonly ICurrentUserService? _currentUser;
    private readonly ICurrentTenant? _currentTenant;
    private Guid? _tenantId;
    public bool IsSeeding { get; set; } = false; // Property to indicate seeding process


    public MasterDbContext(
        DbContextOptions<MasterDbContext> options,
        ICurrentUserService? currentUser,
         ICurrentTenant? currentTenant
        ) : base(options)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }
    public Guid? CurrentTenantId
    {
        get => _tenantId ?? _currentTenant?.TenantId;
        set => _tenantId = value;
    }


    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections { get; set; }
    public DbSet<TenantCategory> TenantCategories { get; set; }
    public DbSet<TenantProvisioningStep> TenantProvisioningSteps { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }


    public DbSet<Language> Languages { get; set; }
    public DbSet<Localization> Localizations { get; set; }


    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<ExternalProvider> ExternalProviders { get; set; }




    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        ChangeTracker.ApplyAuditInformation(CurrentTenantId, _currentUser?.UserId, IsSeeding);
        return await base.SaveChangesAsync(cancellationToken);
    }





    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(MasterDbContext).Assembly,
            x => x.Namespace ==
                 "SamaniCrm.Infrastructure.DbContexts.MasterEntityConfigurations");
        GlobalFilterBuilder.ApplyMasterDbFilters(builder);
    }


}
