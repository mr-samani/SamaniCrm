using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts.MasterEntityConfigurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", "Tenant");

        builder.Property(t => t.Longitude)
              .HasColumnType("decimal(18,8)");

        builder.Property(t => t.Latitude)
                  .HasColumnType("decimal(18,8)");

    }
}
public class TenantCategoryConfiguration : IEntityTypeConfiguration<TenantCategory>
{
    public void Configure(EntityTypeBuilder<TenantCategory> builder)
    {
        builder.ToTable("TenantCategories", "Tenant");

    }
}
public class TenantDatabaseConnectionConfiguration : IEntityTypeConfiguration<TenantDatabaseConnection>
{
    public void Configure(EntityTypeBuilder<TenantDatabaseConnection> builder)
    {
        builder.ToTable("TenantDatabaseConnections", "Tenant");

    }
}
public class TenantProvisioningStepConfiguration : IEntityTypeConfiguration<TenantProvisioningStep>
{
    public void Configure(EntityTypeBuilder<TenantProvisioningStep> builder)
    {
        builder.ToTable("TenantProvisioningSteps", "Tenant");

    }
}
