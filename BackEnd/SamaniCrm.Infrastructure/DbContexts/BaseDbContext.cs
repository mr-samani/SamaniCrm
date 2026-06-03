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

public abstract class BaseDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICurrentUserService? _currentUser;
    private readonly ICurrentTenant? _currentTenant;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    private Guid? _tenantId;
    public bool IsSeeding { get; set; } = false; // Property to indicate seeding process

    public BaseDbContext(DbContextOptions options,
        ICurrentUserService? currentUserService,
        ICurrentTenant? currentTenant,
        IHttpContextAccessor? httpContextAccessor) : base(options)
    {
        _currentUser = currentUserService;
        _currentTenant = currentTenant;
        _httpContextAccessor = httpContextAccessor;
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

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await CreateAuditLogs();
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }





    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);

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
        if (IsSeeding)
        {
            return;
        }
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


    private async Task CreateAuditLogs(CancellationToken cancellationToken = default)
    {
        if (IsSeeding)
        {
            return;
        }
        ChangeTracker.DetectChanges();

        var auditLogs = new List<AuditLog>();

        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                e.Entity is not AppLogEntry &&
                e.State != EntityState.Detached &&
                e.State != EntityState.Unchanged)
            .ToList();

        foreach (var entry in entries)
        {
            var entityType = entry.Metadata.ClrType;

            if (Attribute.IsDefined(entityType, typeof(AuditIgnoreAttribute)))
            {
                continue;
            }
            var correlationId = _httpContextAccessor?.HttpContext?.TraceIdentifier ?? "Unknown";
            var auditLog = new AuditLog
            {
                CorrelationId = correlationId,
                UserId = _currentUser?.UserId,
                DelegatorId = _currentUser?.DelegatorId,
                IsDelegated = _currentUser?.IsDelegated ?? false,
                TenantId = CurrentTenantId,

                EntityName = entityType.Name,

                CreationTime = DateTime.UtcNow
            };

            var key = entry.Metadata.FindPrimaryKey();

            if (key != null)
            {
                auditLog.EntityId = string.Join(
                    ",",
                    key.Properties.Select(
                        p => entry.Property(p.Name).CurrentValue?.ToString()));
            }

            switch (entry.State)
            {
                case EntityState.Added:

                    auditLog.Action = "Create";

                    var createdValues = entry.Properties
                        .Where(p =>
                            !p.Metadata.IsPrimaryKey())
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => new
                            {
                                New = p.CurrentValue
                            });

                    auditLog.Changes =
                        JsonSerializer.Serialize(createdValues);

                    break;

                case EntityState.Modified:

                    auditLog.Action = "Update";

                    var modifiedValues =
                        new Dictionary<string, object>();

                    foreach (var property in entry.Properties)
                    {

                        if (!property.IsModified)
                            continue;

                        if (property.Metadata.PropertyInfo?
                          .IsDefined(typeof(AuditIgnoreAttribute), true) == true)
                        {
                            continue;
                        }

                        if (Equals(
                            property.OriginalValue,
                            property.CurrentValue))
                            continue;

                        modifiedValues[property.Metadata.Name] =
                            new
                            {
                                Old = property.OriginalValue,
                                New = property.CurrentValue
                            };
                    }

                    if (modifiedValues.Count == 0)
                        continue;

                    auditLog.Changes =
                        JsonSerializer.Serialize(modifiedValues);

                    break;

                case EntityState.Deleted:

                    auditLog.Action = "Delete";

                    var deletedValues = entry.Properties
                        .Where(p =>
                            !p.Metadata.IsPrimaryKey())
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => new
                            {
                                Old = p.OriginalValue
                            });

                    auditLog.Changes =
                        JsonSerializer.Serialize(deletedValues);

                    break;
            }

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Count > 0)
        {
            await AuditLogs.AddRangeAsync(
                auditLogs,
                cancellationToken);
        }
    }

}
