using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Services.TenantService;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace SamaniCrm.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUser;
        private readonly Guid? _tenantId;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUser = currentUserService;
        }





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
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);

            builder.Entity<SecuritySetting>(b =>
            {
                b.HasKey(k => k.Id);
            });

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
                b.Property(e => e.FirstName).HasMaxLength(50);
                b.Property(e => e.LastName).HasMaxLength(50);
                b.Property(e => e.Address).HasMaxLength(200);
                b.Property(e => e.PhoneNumber).HasMaxLength(15);
                b.Property(e => e.ProfilePicture).HasMaxLength(200);
            });
            builder.Entity<RolePermission>(b =>
            {
                b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                b.HasOne<ApplicationRole>()
                    .WithMany()
                    .HasForeignKey(rp => rp.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(rp => rp.Permission)
                    .WithMany()
                    .HasForeignKey(rp => rp.PermissionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Permission>()
                .HasIndex(p => p.LocalizeKey)
                .IsUnique();

            builder.Entity<Language>(l =>
            {
                l.HasKey(x => x.Culture);
                l.HasIndex(c => c.Culture).IsUnique();
            });
            builder.Entity<Localization>(l =>
            {
                l.HasKey(k => k.Id);
                l.HasIndex(c => c.Culture);
                l.HasOne(x => x.Language)
                    .WithMany(x => x.Localizations)
                    .HasForeignKey(x => x.Culture)
                    .OnDelete(DeleteBehavior.Cascade);
                l.HasIndex(x => new { x.Key, x.Culture, x.Category }).IsUnique();


                var converter = new ValueConverter<LocalizationCategoryEnum, string>(
                                    v => EnumHelper.GetDescription(v),
                                    v => EnumHelper.GetValueFromDescription<LocalizationCategoryEnum>(v, LocalizationCategoryEnum.Other)
                                );
                l.Property(l => l.Category).HasConversion(converter);
            });


            builder.Entity<Menu>(l =>
            {
                l.HasKey(k => k.Id);
                l.HasMany(c => c.Children)
                    .WithOne()
                    .HasForeignKey(m => m.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                var converter = new ValueConverter<MenuTargetEnum, string>(
                                    v => EnumHelper.GetDescription(v),
                                    v => EnumHelper.GetValueFromDescription<MenuTargetEnum>(v, MenuTargetEnum.Self)
                                );
                l.Property(l => l.Target).HasConversion(converter);
            });
            builder.Entity<MenuTranslation>(t =>
            {
                t.HasKey(t => new { t.MenuId, t.Culture });
                t.HasOne(x => x.Language)
                    .WithMany().HasForeignKey(x => x.Culture)
                    .OnDelete(DeleteBehavior.Cascade);
                t.HasOne(x => x.Menu)
                    .WithMany(x => x.Translations)
                    .HasForeignKey(x => x.MenuId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Tenant>(entity =>
            {
                entity.Property(t => t.Longitude)
                      .HasColumnType("decimal(18,8)");

                entity.Property(t => t.Latitude)
                      .HasColumnType("decimal(18,8)");
            });



            // global filter
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Global filter soft Delete
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                            Expression.Constant(false)
                        ),
                        parameter
                    );
                    entityType.SetQueryFilter(filter);
                }


                // Global filter soft Tenant
                if (typeof(IMayHaveTenant).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(IMayHaveTenant.TenantId)),
                            Expression.Constant(_tenantId)
                            ),
                        parameter
                        );
                    entityType.SetQueryFilter(filter);
                }




            }
        }







        private void ApplyAuditInformation()
        {
            // Apply audit values
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUser.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = DateTime.UtcNow;
                        entry.Entity.ModifiedBy = _currentUser.UserId;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.DeletedBy = _currentUser.UserId;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }
    }
}
