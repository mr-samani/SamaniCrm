using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class IdentityUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users", "identity");
        builder.Property(e => e.FirstName).HasMaxLength(50);
        builder.Property(e => e.LastName).HasMaxLength(50);
        builder.Property(e => e.Address).HasMaxLength(200);
        builder.Property(e => e.PhoneNumber).HasMaxLength(15);
        builder.Property(e => e.ProfilePicture).HasMaxLength(200);

    }
}

public class IdentityRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", "identity");
        builder.HasKey(x => new { x.Name, x.TenantId });
        builder.HasIndex(x => new { x.Id, x.Name, x.TenantId });
    }
}


public class IdentityRolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {

        builder.ToTable("RolePermissions", "identity");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasOne<ApplicationRole>()
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

    }
}

public class IdentityPermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "identity");
        builder.HasIndex(p => p.LocalizeKey)
                .IsUnique();
    }
}




 