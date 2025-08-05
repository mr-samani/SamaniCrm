using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

//___________________________________ تخفیف ها -----------------------------------------------
public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts", "Product");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Value).HasPrecision(18, 2);
    }
}


