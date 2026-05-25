using Microsoft.AspNetCore.Identity;
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
        builder.ToTable("Users", "auth");
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
        builder.ToTable("Roles", "auth");
        builder.HasIndex(x => new { x.Id, x.Name, x.TenantId }).IsUnique();
    }
}
public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("RoleClaims", "auth"); 

    }
}
public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("UserClaims", "auth");

    }
}
public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("UserLogins", "auth");

    }
}
public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("UserRoles", "auth");

    }
}
public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("UserTokens", "auth");

    }
}
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "auth");

    }
}
public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.ToTable("UserSettings", "auth"); 
        builder.HasKey(pc => pc.Id);
        builder.HasOne<ApplicationUser>()               // navigation property in UserSetting
               .WithOne(x => x.UserSetting)       // navigation property in ApplicationUser
               .HasForeignKey<UserSetting>(x => x.UserId) // FK in UserSetting
               .OnDelete(DeleteBehavior.Restrict);


    }
}


public class IdentityRolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {

        builder.ToTable("RolePermissions", "auth");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

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
        builder.ToTable("Permissions", "auth");
        builder.HasIndex(p => p.LocalizeKey)
                .IsUnique();
    }
}




