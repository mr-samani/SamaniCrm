using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Persistence;

public class ApplicationDbInitializer
{
    private readonly ILogger<ApplicationDbInitializer> _logger;
    private readonly MasterDbContext _masterDbcontext;
    private readonly TenantDbContext _tenantDbcontext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly FileManagerSettings _fileManagerSettings;


    public ApplicationDbInitializer(
        ILogger<ApplicationDbInitializer> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<FileManagerSettings> fs,
        MasterDbContext masterDbcontext,
        TenantDbContext tenantDbcontext)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _fileManagerSettings = fs.Value;
        _masterDbcontext = masterDbcontext;
        _tenantDbcontext = tenantDbcontext;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _masterDbcontext.Database.MigrateAsync();
            await _tenantDbcontext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync(Tenant? tenant)
    {
        try
        {
            await TrySeedAsync(tenant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync(Tenant? tenant)
    {
        if (tenant != null)
        {
            Console.WriteLine($"Seed for Tenant= {tenant.Name} - {tenant.Slug} - {tenant.Id}");

        }
        Console.WriteLine("Start Seeding DataBase...");
        setSedding(true);

        await SeedPermissions.TrySeedAsync(_tenantDbcontext);
        await SeedRoles.TrySeedAsync(_tenantDbcontext, _logger, _roleManager);
        // seeld localization must be after seed permissions
        await SeedLocalization.TrySeedAsync(_masterDbcontext,_tenantDbcontext);
        await SeedStaticMenus.TrySeedAsync(_masterDbcontext,_tenantDbcontext);
        await SeedDefaultUsers.TrySeedAsync(_tenantDbcontext, _logger, _userManager, _roleManager);
        await SeedSecuritySettings.TrySeedAsync(_tenantDbcontext);
        await SeedProductCategoriesFromFile.TrySeedAsync(_tenantDbcontext);
        await SeedCurrencies.TrySeedAsync(_tenantDbcontext);
        await SeedEnums.TrySeedAsync(_masterDbcontext);
        await SeedExternalProviders.TrySeedAsync(_masterDbcontext);
        await SeedPages.TrySeedAsync(_tenantDbcontext);
        await SeedDefaultFolders.TrySeedAsync(_tenantDbcontext, _fileManagerSettings);

        setSedding(false);

    }


    private void setSedding(bool seeding)
    {
        _tenantDbcontext.IsSeeding = seeding;
        _masterDbcontext.IsSeeding = seeding;
    }
}
