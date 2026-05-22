using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(x => x.Culture);
        builder.HasIndex(c => c.Culture).IsUnique();
    }
}

public class LocalizationConfiguration : IEntityTypeConfiguration<Localization>
{
    public void Configure(EntityTypeBuilder<Localization> builder)
    {
        builder.HasKey(k => k.Id);
        builder.HasIndex(c => c.Culture);
        builder.HasOne(x => x.Language)
             .WithMany(x => x.Localizations)
             .HasForeignKey(x => x.Culture)
             .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.Key, x.Culture, x.Category }).IsUnique();


        var converter = new ValueConverter<LocalizationCategoryEnum, string>(
                            v => EnumHelper.GetDescription(v),
                            v => EnumHelper.GetValueFromDescription<LocalizationCategoryEnum>(v, LocalizationCategoryEnum.Other)
                        );
        builder.Property(l => l.Category).HasConversion(converter);
    }
}

