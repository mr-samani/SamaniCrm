using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

//___________________________________ واحد ارزی -----------------------------------------------
public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies", "product");
        builder.HasKey(pc => pc.Id);
        builder.HasAlternateKey(c => c.CurrencyCode);
        builder.HasIndex(h => h.CurrencyCode).IsUnique();

        builder.Property(p => p.ExchangeRateToBase).HasPrecision(18, 2);

        builder.HasMany(p => p.ProductPrices)
            .WithOne(c => c.Currency)
            .HasForeignKey(p => p.CurrencyCode)
            .HasPrincipalKey(c => c.CurrencyCode)
            .OnDelete(DeleteBehavior.Cascade);

    }
}


