using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.Subscription;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity;
using System.Linq.Expressions;

namespace SamaniCrm.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;
    private readonly ICurrentTenant? _currentTenant;

    private Guid? _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService,
        ICurrentTenant? currentTenant) : base(options)
    {
        _currentUser = currentUserService;
        _currentTenant = currentTenant;
    }



    public Guid? CurrentTenantId
    {
        get => _tenantId ?? _currentTenant?.TenantId;
        set => _tenantId = value;
    }


    public DbSet<TenantLogSetting> TenantLogSettings { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }


    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections { get; set; }
    public DbSet<TenantCategory> TenantCategories { get; set; }
    public DbSet<TenantProvisioningStep> TenantProvisioningSteps { get; set; }



    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Localization> Localizations { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuTranslation> MenuTranslations { get; set; }
    public DbSet<SecuritySetting> SecuritySettings { get; set; }
    public DbSet<UserSetting> UserSetting { get; set; }

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

    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<ExternalProvider> ExternalProviders { get; set; }

    #region Dashboard
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<DashboardItem> DashboardItems { get; set; }
    #endregion


    #region Subscription

    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanTranslation> PlanTranslations { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<PlanPrice> PlanPrices { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AddOn> AddOns { get; set; }
    public DbSet<SubscriptionAddOn> SubscriptionAddOns { get; set; }

    #endregion

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }





    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        SetGLobalFilter(builder);
    }



    /// <summary>
    /// Delete Global filter
    /// </summary>       
    private void SetGLobalFilter(ModelBuilder builder)
    {

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            Expression? finalExpression = null;

            // Soft Delete filter
            var isDeletedProp = entityType.ClrType.GetProperty("IsDeleted");
            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                var isDeletedExpression = Expression.Equal(
                    Expression.Property(parameter, isDeletedProp),
                    Expression.Constant(false)
                );

                finalExpression = isDeletedExpression;
            }

            // Tenant filter
            if (typeof(IMayHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                // ─── ساخت expression برای this.CurrentTenantId ───
                var tenantIdProperty = Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentTenantId)
                );

                var tenantExpression = Expression.Equal(
                    Expression.Property(parameter, nameof(IMayHaveTenant.TenantId)),
                    tenantIdProperty
                );


                finalExpression = finalExpression == null
                    ? tenantExpression
                    : Expression.AndAlso(finalExpression, tenantExpression);
            }

            if (finalExpression != null)
            {
                var lambda = Expression.Lambda(finalExpression, parameter);
                // Console.WriteLine(lambda.Body);
                entityType.SetQueryFilter(lambda);
            }
        }
    }




    private void ApplyAuditInformation()
    {
        var currentUserId = _currentUser?.UserId;

        // فقط یک پیمایش روی هم entity ها
        var entries = ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached &&
                        e.State != EntityState.Unchanged)
            .ToList();

        foreach (var entry in entries)
        {
            // --- TenantId فقط برای entity های جدید ---
            if (entry.Entity is IMayHaveTenant tenantEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    // فقط اگر قبلاً ست نشده باشد
                    if (tenantEntity.TenantId == default)
                    {
                        tenantEntity.TenantId = CurrentTenantId;
                    }
                }
            }

            // --- Audit Fields ---
            if (entry.Entity is IAuditedEntity auditedEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditedEntity.CreatedAt = DateTime.UtcNow;
                        auditedEntity.CreatedBy = currentUserId;
                        break;

                    case EntityState.Modified:
                        auditedEntity.ModifiedAt = DateTime.UtcNow;
                        auditedEntity.ModifiedBy = currentUserId;

                        if (auditedEntity.IsDeleted)
                        {
                            auditedEntity.DeletedAt = DateTime.UtcNow;
                            auditedEntity.DeletedBy = currentUserId;
                        }
                        break;

                    case EntityState.Deleted:
                        // تبدیل Hard Delete به Soft Delete
                        auditedEntity.IsDeleted = true;
                        auditedEntity.DeletedAt = DateTime.UtcNow;
                        auditedEntity.DeletedBy = currentUserId;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }
    }
}
