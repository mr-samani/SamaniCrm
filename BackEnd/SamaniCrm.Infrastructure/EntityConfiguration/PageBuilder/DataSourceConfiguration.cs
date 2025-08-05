using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities.PageBuilderEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SamaniCrm.Infrastructure.EntityConfiguration.PageBuilder;

public class DataSourceConfiguration : IEntityTypeConfiguration<DataSource>
{
    public void Configure(EntityTypeBuilder<DataSource> builder)
    {
        builder.ToTable("DataSources", "DS");
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Fields)
            .WithOne(x => x.DataSource)
            .HasForeignKey(x => x.DataSourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
public class DataSourceFieldConfiguration : IEntityTypeConfiguration<DataSourceField>
{
    public void Configure(EntityTypeBuilder<DataSourceField> builder)
    {
        builder.ToTable("DataSourceFields", "DS");
        builder.HasKey(x => x.Id);
    }
}