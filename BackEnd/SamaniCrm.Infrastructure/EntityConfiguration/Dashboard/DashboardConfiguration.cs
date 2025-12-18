using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.Dashboard;

namespace SamaniCrm.Infrastructure.EntityConfiguration.PageBuilder;

public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable("Dashboards", "panel");
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.DashboardItems)
            .WithOne(x => x.Dashboard)
            .HasForeignKey(x => x.DashboardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


public class DashboardItemConfihuration : IEntityTypeConfiguration<DashboardItem>
{
    public void Configure(EntityTypeBuilder<DashboardItem> builder)
    {
        builder.ToTable("DashboardItems", "panel");
        builder.HasKey(x => x.Id);


    }
}