using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class PageTranslationConfiguration : IEntityTypeConfiguration<PageTranslation>
{
    public void Configure(EntityTypeBuilder<PageTranslation> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasKey(p => new { p.PageId, p.Culture });

        builder.HasOne(p => p.Page)
            .WithMany(p => p.Translations)
            .HasForeignKey(p => p.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Language)
            .WithMany()
            .HasForeignKey(p => p.Culture)
            .OnDelete(DeleteBehavior.Cascade);


    }
}

