using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity;
using RefreshToken = SamaniCrm.Domain.Entities.RefreshToken;

namespace SamaniCrm.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;


        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Localization> Localizations { get; set; }


        // public DbSet<RolePermission> RolePermissions { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }


        public override int SaveChanges()
        {
            ApplyAuditInformation();
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            ApplySoftDelete();
            return await base.SaveChangesAsync(cancellationToken);
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
                l.HasOne(x=>x.Language)
                    .WithMany(x=>x.Localizations)
                    .HasForeignKey(x => x.Culture)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                            Expression.Constant(false)
                        ),
                        parameter
                    );
                    entityType.SetQueryFilter(filter);
                }
            }
        }







        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreationTime = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedTime = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
        }


        private void ApplySoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified; // دیگه Delete واقعی نمیشه
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedTime = DateTime.UtcNow;
                    if (_currentUserService.UserId != null)
                        entry.Entity.DeletedBy = _currentUserService.UserId;
                }
            }
        }



    }
}
