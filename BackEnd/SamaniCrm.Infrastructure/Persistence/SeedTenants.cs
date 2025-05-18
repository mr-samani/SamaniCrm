using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedTenants
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            var defaultTenant = dbContext.Tenants.Where(w => w.Name == "Default").FirstOrDefault();


            if (defaultTenant == null)
            {
                defaultTenant = new Tenant()
                {
                    Name = "Default",
                    IsActive = true,
                    Slug = "Default"
                };
                await dbContext.Tenants.AddAsync(defaultTenant);
                await dbContext.SaveChangesAsync();
            }

            await SeedProductCategoriesFromFile.TrySeedAsync(dbContext, defaultTenant.Id);

        }
    }
}
