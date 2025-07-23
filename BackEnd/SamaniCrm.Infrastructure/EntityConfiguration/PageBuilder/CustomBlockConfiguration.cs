using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities.PageBuilderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.EntityConfiguration.PageBuilder;

public class CustomBlockConfiguration : IEntityTypeConfiguration<CustomBlock>
{
    public void Configure(EntityTypeBuilder<CustomBlock> builder)
    {
        builder.ToTable("CustomBlocks", "Bldr");
        builder.HasKey(x => x.Id);
    }
}
