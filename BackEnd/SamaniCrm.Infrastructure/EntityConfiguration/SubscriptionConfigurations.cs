using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.Subscription;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.EntityConfiguration;


public class PlanConfigurations : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans", "subscription");
    }
}
public class PlanFeatureConfigurations : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.ToTable("PlanFeatures", "subscription");
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}
public class PlanPriceConfigurations : IEntityTypeConfiguration<PlanPrice>
{
    public void Configure(EntityTypeBuilder<PlanPrice> builder)
    {
        builder.ToTable("PlanPrices", "subscription");
        builder.Property(t => t.Amount)
              .HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}
public class AddOnConfigurations : IEntityTypeConfiguration<AddOn>
{
    public void Configure(EntityTypeBuilder<AddOn> builder)
    {
        builder.ToTable("AddOns", "subscription");
        builder.Property(t => t.UnitPrice)
              .HasColumnType("decimal(18,2)");
        builder.Property(t => t.Quantity)
              .HasColumnType("decimal(18,2)");
    }
}


public class SubscriptionConfigurations : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions", "subscription");
        builder.Property(t => t.TotalPrice)
              .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.PlanPrice).WithMany().HasForeignKey(x => x.PriceId).OnDelete(DeleteBehavior.Cascade);
    }
}




public class SubscriptionAddOnConfigurations : IEntityTypeConfiguration<SubscriptionAddOn>
{
    public void Configure(EntityTypeBuilder<SubscriptionAddOn> builder)
    {
        builder.ToTable("SubscriptionAddOns", "subscription");
        builder.Property(t => t.UnitPrice)
              .HasColumnType("decimal(18,2)");
        builder.Property(t => t.Quantity)
              .HasColumnType("decimal(18,2)");
        builder.Property(t => t.TotalPrice)
              .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Subscription).WithMany().HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AddOn).WithMany().HasForeignKey(x => x.AddOnId).OnDelete(DeleteBehavior.Cascade);
    }
}



