using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts", "product");
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.CartItems)
             .WithOne(x => x.Cart)
             .HasForeignKey(k => k.CartId)
             .OnDelete(DeleteBehavior.Cascade);


    }
}



public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", "product");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductUnitPrice)
            .HasPrecision(18, 2);
    }
}