using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;
using RefreshToken = SamaniCrm.Domain.Entities.RefreshToken;

namespace SamaniCrm.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    => base.SaveChangesAsync(cancellationToken);


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

            builder.Entity<Permission>(b =>
            {
                b.HasKey(b => b.Id);
                b.Property(b => b.Name).IsRequired().HasMaxLength(256);
                b.Property(b => b.DisplayName).IsRequired().HasMaxLength(256);

                b.HasOne(b => b.Parent)
                    .WithMany(b => b.Children)
                    .HasForeignKey(b => b.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(b => b.Name).IsUnique();
            });

            builder.Entity<RolePermission>(rp =>
            {
                rp.HasKey(p => new { p.RoleId, p.PermissionId });

                rp.HasOne<ApplicationRole>()
                   .WithMany(r => r.RolePermissions)
                   .HasForeignKey(x => x.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

                rp.HasOne(x => x.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(x => x.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            //builder.Entity<UserRole>(ur =>
            //{
            //    ur.HasMany<ApplicationRole>()
            //    .WithMany(x => x.Users)
            //    .HasForeignKey(x => x.UserId)
            //    .hasForeignKey(x => x.UserId);

            //});


        }
    }
}
