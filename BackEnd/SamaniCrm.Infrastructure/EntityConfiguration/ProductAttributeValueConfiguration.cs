using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

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


