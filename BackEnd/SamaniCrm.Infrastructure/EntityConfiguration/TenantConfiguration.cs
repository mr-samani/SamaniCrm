using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {

        builder.Property(t => t.Longitude)
              .HasColumnType("decimal(18,8)");

        builder.Property(t => t.Latitude)
                  .HasColumnType("decimal(18,8)");

    }
}
