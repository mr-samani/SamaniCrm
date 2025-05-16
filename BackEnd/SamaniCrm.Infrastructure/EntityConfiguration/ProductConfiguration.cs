using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.SKU).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.HasIndex(p => new { p.TenantId, p.SKU }).IsUnique();

            builder.HasOne(p => p.Tenant)
                   .WithMany(t => t.Products)
                   .HasForeignKey(p => p.TenantId);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.ProductType)
                   .WithMany()
                   .HasForeignKey(p => p.ProductTypeId);
        }
    }

}
