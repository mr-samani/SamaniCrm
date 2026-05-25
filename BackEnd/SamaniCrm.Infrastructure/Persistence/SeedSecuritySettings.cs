using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedSecuritySettings
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {

            var settings = await dbContext.SecuritySettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                Console.WriteLine("Try seed password complexity data");
                var securitySetting = new SecuritySetting()
                {
                    RequireDigit = true,
                    RequireLowercase = false,
                    RequiredLength = 6,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false,
                };
                await dbContext.SecuritySettings.AddAsync(securitySetting);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("end of password complexity data");
            }
        }
    }
}
