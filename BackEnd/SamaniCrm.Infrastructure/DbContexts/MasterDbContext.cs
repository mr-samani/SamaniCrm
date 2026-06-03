using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts;

public class MasterDbContext : BaseDbContext, IMasterDbContext
{
    public MasterDbContext(
        DbContextOptions<MasterDbContext> options,
        ICurrentUserService? currentUserService,
        ICurrentTenant? currentTenant,
        IHttpContextAccessor? httpContextAccessor) : base(options, currentUserService, currentTenant, httpContextAccessor)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections { get; set; }
    public DbSet<TenantCategory> TenantCategories { get; set; }
    public DbSet<TenantProvisioningStep> TenantProvisioningSteps { get; set; }

    public DbSet<Language> Languages { get; set; }
    public DbSet<Localization> Localizations { get; set; }


    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<ExternalProvider> ExternalProviders { get; set; }



}
