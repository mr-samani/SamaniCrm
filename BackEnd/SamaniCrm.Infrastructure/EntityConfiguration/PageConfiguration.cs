using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.EntityConfiguration;
public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasOne<ApplicationUser>()
             .WithMany()
             .HasForeignKey(x => x.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);


        var converter = new ValueConverter<PageTypeEnum, string>(
                            v => EnumHelper.GetDescription(v),
                            v => EnumHelper.GetValueFromDescription<PageTypeEnum>(v, PageTypeEnum.OtherPages)
                        );
        builder.Property(l => l.Type).HasConversion(converter);
    }
}



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




