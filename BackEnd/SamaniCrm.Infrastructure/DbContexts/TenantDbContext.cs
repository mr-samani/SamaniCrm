using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Domain.Attributes;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.Subscription;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.DbContexts;

public class TenantDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;
    private readonly ICurrentTenant? _currentTenant;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly IAuditLogFactory? _auditFactory;

    private Guid? _tenantId;
    // for dont create audit log
    public bool IsSeeding { get; set; } = false; // Property to indicate seeding process

    public TenantDbContext(DbContextOptions options,
         IAuditLogFactory? auditFactory,
         ICurrentUserService? currentUserService,
         ICurrentTenant? currentTenant,
         IHttpContextAccessor? httpContextAccessor
      ) : base(options)
    {
        _currentUser = currentUserService;
        _currentTenant = currentTenant;
        _httpContextAccessor = httpContextAccessor;
        _auditFactory = auditFactory;
    }



    public Guid? CurrentTenantId
    {
        get => _tenantId ?? _currentTenant?.TenantId;
        set => _tenantId = value;
    }


    public DbSet<TenantAppLogSetting> TenantLogSettings { get; set; }
    public DbSet<AppLogEntry> LogEntries { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SecurityLogEntry> SecurityLogEntries { get; set; }




    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }


    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuTranslation> MenuTranslations { get; set; }
    public DbSet<SecuritySetting> SecuritySettings { get; set; }
    public DbSet<UserSetting> UserSetting { get; set; }
    public DbSet<UserDelegation> UserDelegations { get; set; }

    public DbSet<Page> Pages { get; set; }
    public DbSet<PageTranslation> PageTranslations { get; set; }


    #region Products

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductCategoryTranslation> ProductCategoryTranslations { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductTranslation> ProductTranslations { get; set; }

    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<ProductTypeTranslation> ProductTypeTranslations { get; set; }

    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<ProductAttributeTranslation> ProductAttributeTranslations { get; set; }
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductFile> ProductFiles { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Discount> Discount { get; set; }
    #endregion 

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<FileFolder> FileFolders { get; set; }


    #region Dashboard
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<DashboardItem> DashboardItems { get; set; }
    #endregion


    #region Subscription

    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanTranslation> PlanTranslations { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<PlanFeatureTranslation> PlanFeatureTranslations { get; set; }
    public DbSet<PlanPrice> PlanPrices { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AddOn> AddOns { get; set; }
    public DbSet<AddOnTranslation> AddOnTranslations { get; set; }
    public DbSet<SubscriptionAddOn> SubscriptionAddOns { get; set; }




    #endregion




    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await CreateAuditLogs(cancellationToken);

        ChangeTracker.ApplyAuditInformation(
            CurrentTenantId,
            _currentUser?.UserId,
            IsSeeding);
        return await base.SaveChangesAsync(cancellationToken);
    }





    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(TenantDbContext).Assembly,
            x => x.Namespace ==
                 "SamaniCrm.Infrastructure.DbContexts.TenantEntityConfigurations");
        GlobalFilterBuilder.ApplyFilters(builder, this);
    }





    private async Task CreateAuditLogs(CancellationToken cancellationToken)
    {
        if (_auditFactory == null || IsSeeding == true)
        {
            return;
        }
        var logs =
            _auditFactory.Create(
                ChangeTracker,
                new AuditContext
                {
                    UserId = _currentUser?.UserId,
                    DelegatorId = _currentUser?.DelegatorId,
                    TenantId = CurrentTenantId,
                    IsDelegated =
                        _currentUser?.IsDelegated ?? false,
                    CorrelationId =
                        _httpContextAccessor
                        ?.HttpContext
                        ?.TraceIdentifier ?? "Unknown"
                });

        if (logs.Count > 0)
        {
            await AuditLogs.AddRangeAsync(logs, cancellationToken);
        }
    }


}
