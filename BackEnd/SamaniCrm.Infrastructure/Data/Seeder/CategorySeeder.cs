using Microsoft.Extensions.Logging;
using MimeDetective.Storage;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Data.Seeder;
public class CategorySeeder : TenantDataSeederBase<ApplicationDbContext>
{
    public override int Order => 1;

    public CategorySeeder(
        ILogger<CategorySeeder> logger,
        ITenantDbContextFactory contextFactory)
        : base(logger, contextFactory)
    {
    }
    protected override async Task SeedAsync(ApplicationDbContext context, ITenant tenant, CancellationToken cancellation)
    {
          var categories = new[]
        {
            new TenantCategory
            {
                Id = Guid.NewGuid(),
                Name = "محصولات عمومی",
                Description = "محصولات عمومی و پرکاربرد",
                ParentId = null,
                SortOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = tenant.Id,
                TenantId = tenant.Id
            },
            new TenantCategory
            {
                Id = Guid.NewGuid(),
                Name = "خدمات",
                Description = "انواع خدمات",
                ParentId = null,
                SortOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = tenant.Id,
                TenantId = tenant.Id
            },
            new TenantCategory
            {
                Id = Guid.NewGuid(),
                Name = "تجهیزات",
                Description = "تجهیزات و ملزومات",
                ParentId = null,
                SortOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = tenant.Id,
                TenantId = tenant.Id
            }
        };

        await context.TenantCategories.AddRangeAsync(categories, cancellation);
        await context.SaveChangesAsync(cancellation);

        _logger.LogInformation("Seeded {Count} categories for tenant {TenantId}",
            categories.Length, tenant.Id);
    }

  
}