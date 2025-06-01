using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;
public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasMany(p => p.Children)
            .WithOne(p => p.Parent)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}

public class ProductCategoryTranslationConfiguration : IEntityTypeConfiguration<ProductCategoryTranslation>
{
    public void Configure(EntityTypeBuilder<ProductCategoryTranslation> builder)
    {
        builder.ToTable("ProductCategoryTranslations", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.ProductCategory)
            .WithMany(c => c.Translations)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Language)
                 .WithMany().HasForeignKey(x => x.Culture)
                 .OnDelete(DeleteBehavior.Cascade);

    }
}