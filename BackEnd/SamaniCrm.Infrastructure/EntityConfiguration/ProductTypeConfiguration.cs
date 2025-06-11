using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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