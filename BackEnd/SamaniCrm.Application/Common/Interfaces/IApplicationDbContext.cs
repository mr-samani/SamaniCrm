using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.PageBuilderEntities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuEntity = SamaniCrm.Domain.Entities.Menu;
using ProductEntity = SamaniCrm.Domain.Entities.ProductEntities.Product;
using SecuritySettingEntity = SamaniCrm.Domain.Entities.SecuritySetting;


namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<MenuEntity> Menus { get; set; }
        public DbSet<MenuTranslation> MenuTranslations { get; set; }
        public DbSet<SecuritySettingEntity> SecuritySettings { get; set; }
        public DbSet<UserSetting> UserSetting { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageTranslation> PageTranslations { get; set; }


        #region Products
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCategoryTranslation> ProductCategoryTranslations { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
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



        public DbSet<CustomBlock> CustomBlocks { get; set; }
        public DbSet<ExternalProvider> ExternalProviders { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
