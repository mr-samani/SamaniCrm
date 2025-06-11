using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{

    //---------------------------پیکربندی جداول مربوط به محصولات--------------------------------------
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "product");
        builder.HasKey(p => p.Id);


        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);


        builder.OwnsOne(p => p.SKU, sku =>
        {
            sku.Property(p => p.Value)
                .HasColumnName("SKU")
                .HasMaxLength(1000)
                .IsRequired();
        });

        builder.HasOne(p => p.ProductType)
              .WithMany(t => t.Products)
              .HasForeignKey(p => p.ProductTypeId)
              .OnDelete(DeleteBehavior.Restrict)
              .IsRequired();


    }
}

//------------------- ترجمه عنوان و توضیحات محصول / کالا
public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> builder)
    {
        builder.ToTable("ProductTranslations", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.Product)
            .WithMany(c => c.Translations)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Language)
                 .WithMany().HasForeignKey(x => x.Culture)
                 .OnDelete(DeleteBehavior.Cascade);

    }
}


