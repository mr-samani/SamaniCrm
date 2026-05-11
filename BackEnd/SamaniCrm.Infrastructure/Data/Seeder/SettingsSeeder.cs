using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Data.Seeder;

public class SettingsSeeder : TenantDataSeederBase<ApplicationDbContext>
{
    public override int Order => 2;

    public SettingsSeeder(
        ILogger<SettingsSeeder> logger,
        ITenantDbContextFactory contextFactory)
        : base(logger, contextFactory)
    {
    }

    protected override async Task SeedAsync(ApplicationDbContext context, ITenant tenant, CancellationToken cancellation)
    {
  
        var settings = new[]
        {
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "General",
                Key = "CompanyName",
                Value = tenant.Name,
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "General",
                Key = "Currency",
                Value = "IRR",
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "General",
                Key = "Timezone",
                Value = "Asia/Tehran",
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Invoice",
                Key = "InvoicePrefix",
                Value = "INV",
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Invoice",
                Key = "InvoiceFooter",
                Value = "با تشکر از خرید شما",
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Email",
                Key = "SmtpHost",
                Value = "",
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Email",
                Key = "SmtpPort",
                Value = "587",
                ValueType = TenantSettingValueType.Number
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Email",
                Key = "FromEmail",
                Value = tenant.Email,
                ValueType = TenantSettingValueType.String
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Features",
                Key = "EnableInvoicing",
                Value = "true",
                ValueType = TenantSettingValueType.Boolean
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Features",
                Key = "EnableInventory",
                Value = "true",
                ValueType = TenantSettingValueType.Boolean
            },
            new TenantSetting
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Category = "Features",
                Key = "EnableCustomers",
                Value = "true",
                ValueType = TenantSettingValueType.Boolean
            }
        };

        await context.TenantSettings.AddRangeAsync(settings, cancellation);
        await context.SaveChangesAsync(cancellation);

        _logger.LogInformation("Seeded {Count} settings for tenant {TenantId}",
            settings.Length, tenant.Id);
    }

 
}