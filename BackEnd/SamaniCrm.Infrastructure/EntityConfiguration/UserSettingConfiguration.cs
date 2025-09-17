using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.EntityConfiguration;

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        // builder.ToTable("UserSettings", "dbo");
        builder.HasKey(pc => pc.Id);
        builder.HasOne<ApplicationUser>()               // navigation property in UserSetting
               .WithOne(x => x.UserSetting)       // navigation property in ApplicationUser
               .HasForeignKey<UserSetting>(x => x.UserId) // FK in UserSetting
               .OnDelete(DeleteBehavior.Restrict);



    }
}
