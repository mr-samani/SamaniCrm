using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Persistence;

public class ApplicationDbInitializer
{
    private readonly ILogger<ApplicationDbInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly FileManagerSettings _fileManagerSettings;


    public ApplicationDbInitializer(
        ILogger<ApplicationDbInitializer> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<FileManagerSettings> fs
        )
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _fileManagerSettings = fs.Value;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
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
        _context.IsSeeding = true;

        await SeedPermissions.TrySeedAsync(_context);
        await SeedRoles.TrySeedAsync(_context, _logger, _roleManager);
        // seeld localization must be after seed permissions
        await SeedLocalization.TrySeedAsync(_context);
        await SeedStaticMenus.TrySeedAsync(_context);
        await SeedDefaultUsers.TrySeedAsync(_context, _logger, _userManager, _roleManager);
        await SeedSecuritySettings.TrySeedAsync(_context);
        await SeedProductCategoriesFromFile.TrySeedAsync(_context);
        await SeedCurrencies.TrySeedAsync(_context);
        await SeedEnums.TrySeedAsync(_context);
        await SeedExternalProviders.TrySeedAsync(_context);
        await SeedPages.TrySeedAsync(_context);
        await SeedDefaultFolders.TrySeedAsync(_context, _fileManagerSettings);

        _context.IsSeeding = false;

    }
}
