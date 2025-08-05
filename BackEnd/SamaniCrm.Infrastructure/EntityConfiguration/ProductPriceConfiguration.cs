using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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


