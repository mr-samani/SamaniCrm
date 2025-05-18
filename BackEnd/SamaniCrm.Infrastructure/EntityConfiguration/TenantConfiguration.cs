using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(pc => pc.Id);
           
    }
}