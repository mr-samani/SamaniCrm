using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Core.Shared.Helpers;
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
              .WithMany(t=>t.Products)
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
//___________________________________نوع کالا_____________________________________________________________
public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder.ToTable("ProductTypes", "product");
        builder.HasKey(pc => pc.Id);
    }
}
public class ProductTypeTranslationConfiguration : IEntityTypeConfiguration<ProductTypeTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTypeTranslation> builder)
    {
        builder.ToTable("ProductTypeTranslations", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.ProductType)
            .WithMany(c => c.Translations)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Language)
                 .WithMany().HasForeignKey(x => x.Culture)
                 .OnDelete(DeleteBehavior.Cascade);

    }
}
//_______________________________________ویژگی های کالا_________________________________________________________

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.ProductType)
            .WithMany(c => c.Attributes)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.Restrict);


        var converter = new ValueConverter<ProductAttributeDataTypeEnum, string>(
                            v => EnumHelper.GetDescription(v),
                            v => EnumHelper.GetValueFromDescription<ProductAttributeDataTypeEnum>(v, ProductAttributeDataTypeEnum.String)
                        );
        builder.Property(b => b.DataType).HasConversion(converter);
    }
}

//_______________________________________ترجمه ویژگی های کالا_________________________________________________________
public class ProductAttributeTranslationConfiguration : IEntityTypeConfiguration<ProductAttributeTranslation>
{
    public void Configure(EntityTypeBuilder<ProductAttributeTranslation> builder)
    {
        builder.ToTable("ProductAttributeTranslations", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.ProductAttribute)
            .WithMany(c => c.Translations)
            .HasForeignKey(p => p.ProductAttributeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Language)
                 .WithMany().HasForeignKey(x => x.Culture)
                 .OnDelete(DeleteBehavior.Cascade);

    }
}
//________________________________مقادیر ویژگی های کالا____________________________________________________

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.Product)
            .WithMany(c => c.AttributeValues)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Attribute)
                 .WithMany(c => c.AttributeValues).HasForeignKey(x => x.AttributeId)
                 .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(p => p.Value, attributeValue =>
        {
            attributeValue.Property(p => p.Value)
                .HasColumnName("AttributeValue")
                .HasMaxLength(100)
                .IsRequired();
        });

    }
}

//________________________________ تصاویر کالا____________________________________________________

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.Product)
            .WithMany(c => c.Images)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

//________________________________فایل های  کالا____________________________________________________

public class ProductFileConfiguration : IEntityTypeConfiguration<ProductFile>
{
    public void Configure(EntityTypeBuilder<ProductFile> builder)
    {
        builder.ToTable("ProductFiles", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasOne(p => p.Product)
            .WithMany(c => c.Files)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
//________________________________قیمت های  کالا____________________________________________________
public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.ToTable("ProductPrices", "product");
        builder.HasKey(pc => pc.Id);
        builder.Property(p => p.Price)
            .HasPrecision(18, 2); // 18 رقم با 2 رقم اعشار
        builder.HasOne(p => p.Product)
            .WithMany(c => c.Prices)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}