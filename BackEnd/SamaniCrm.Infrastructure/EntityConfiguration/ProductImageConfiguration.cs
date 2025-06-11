using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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


        builder.HasAlternateKey(p => p.FileId);
        builder.HasIndex(p => p.FileId).IsUnique();

        builder.HasOne(p => p.File)
            .WithMany()
            .HasForeignKey(p => p.FileId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}


