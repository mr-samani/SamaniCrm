using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(k => k.Id);
        builder.HasMany(c => c.Children)
            .WithOne()
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        var converter = new ValueConverter<MenuTargetEnum, string>(
                            v => EnumHelper.GetDescription(v),
                            v => EnumHelper.GetValueFromDescription<MenuTargetEnum>(v, MenuTargetEnum.Self)
                        );
        builder.Property(l => l.Target).HasConversion(converter);
    }
}



public class MenuTranslationConfiguration : IEntityTypeConfiguration<MenuTranslation>
{
    public void Configure(EntityTypeBuilder<MenuTranslation> builder)
    {

        builder.HasKey(t => new { t.MenuId, t.Culture });
        builder.HasOne(x => x.Language)
            .WithMany().HasForeignKey(x => x.Culture)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Menu)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
