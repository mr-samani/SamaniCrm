using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecuritySettingEntity = SamaniCrm.Domain.Entities.SecuritySetting;


namespace SamaniCrm.Application.Common.Interfaces;

public interface IApplicationDbContext
{  
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }
    public DbSet<TenantDatabaseConnection> TenantDatabaseConnections { get; set; }
    public DbSet<TenantCategory> TenantCategories { get; set; }
    public DbSet<ProvisioningStep> ProvisioningSteps { get; set; }



    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Localization> Localizations { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuTranslation> MenuTranslations { get; set; }
    public DbSet<SecuritySettingEntity> SecuritySettings { get; set; }
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


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
