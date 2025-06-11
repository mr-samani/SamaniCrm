using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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
