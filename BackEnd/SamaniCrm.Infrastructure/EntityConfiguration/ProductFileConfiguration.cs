using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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

        builder.HasAlternateKey(p => p.FileId);
        builder.HasIndex(p => p.FileId).IsUnique();

        builder.HasOne(p => p.File)
            .WithMany()
            .HasForeignKey(p => p.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


