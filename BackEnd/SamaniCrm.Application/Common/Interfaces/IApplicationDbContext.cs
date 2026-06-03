using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.Subscription;
using SecuritySettingEntity = SamaniCrm.Domain.Entities.SecuritySetting;


namespace SamaniCrm.Application.Common.Interfaces;


public interface IApplicationDbContext
{
    public DatabaseFacade Database { get; }

    public DbSet<TenantAppLogSetting> TenantLogSettings { get; set; }
    public DbSet<AppLogEntry> LogEntries { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SecurityLogEntry> SecurityLogEntries { get; set; }


    public DbSet<TenantSetting> TenantSettings { get; set; }



    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuTranslation> MenuTranslations { get; set; }
    public DbSet<SecuritySettingEntity> SecuritySettings { get; set; }
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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
