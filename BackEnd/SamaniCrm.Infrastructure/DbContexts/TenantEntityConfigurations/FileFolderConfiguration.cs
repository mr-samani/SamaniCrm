using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.DbContexts.TenantEntityConfigurations
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
