using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "product");
        builder.HasKey(p => p.Id);

       // builder.HasIndex(p => new { p.TenantId, p.SKU }).IsUnique();

        //builder.HasOne(p => p.Tenant)
        //       .WithMany(t => t.Products)
        //       .HasForeignKey(p => p.TenantId);

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId);

        //builder.HasOne(p => p.ProductType)
        //       .WithMany()
        //       .HasForeignKey(p => p.ProductTypeId);
       // builder.ComplexProperty(p => p.SKU);
        builder.OwnsOne(p => p.SKU, sa =>
        {
            sa.Property(p => p.Value)
                .HasColumnName("SKU")
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}

public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> builder)
    {
        builder.ToTable("ProductTranslations", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.Product)
            .WithMany(c => c.Translations)
            .HasForeignKey(p => p.Id)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Language)
                 .WithMany().HasForeignKey(x => x.Culture)
                 .OnDelete(DeleteBehavior.Cascade);

    }
}
