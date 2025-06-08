using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.EntityConfiguration
{
    public class FileFolderConfiguration : IEntityTypeConfiguration<FileFolder>
    {
        public void Configure(EntityTypeBuilder<FileFolder> builder)
        {
            builder.ToTable("FileFolders", "file");
            builder.HasKey(pc => pc.Id);
            builder.HasMany(p => p.Children)
                .WithOne(p => p.Parent)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.ByteSize)
                    .HasPrecision(20, 10); 

        }
    }

}
