using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class SecuritySettingConfiguration : IEntityTypeConfiguration<SecuritySetting>
{
    public void Configure(EntityTypeBuilder<SecuritySetting> builder)
    {
        builder.HasKey(k => k.Id);
    }
}

