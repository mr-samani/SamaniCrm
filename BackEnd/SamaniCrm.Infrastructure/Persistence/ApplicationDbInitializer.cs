using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Security;

namespace SamaniCrm.Infrastructure.Persistence;

public class ApplicationDbInitializer
{
    private readonly ILogger<ApplicationDbInitializer> _logger;
    private readonly MasterDbContext _masterDbcontext;
    private readonly TenantDbContext _hostDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly FileManagerSettings _fileManagerSettings;

    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;


    public ApplicationDbInitializer(
        ILogger<ApplicationDbInitializer> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<FileManagerSettings> fs,
        MasterDbContext masterDbcontext,
        ITenantDbContextFactory tenantDbContextFactory,
        ITenantDatabaseService tenantDatabaseService,
        TenantDbContext hostDbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _fileManagerSettings = fs.Value;
        _masterDbcontext = masterDbcontext;
        _tenantDbContextFactory = tenantDbContextFactory;
        _tenantDatabaseService = tenantDatabaseService;
        _hostDbContext = hostDbContext;
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
        var ConnectionString = _tenantDatabaseService.GetConnectionString(tenant?.Id);
        using var _tenantDbcontext = _tenantDbContextFactory.Create(ConnectionString!);
        _masterDbcontext.IsSeeding = true;
        _tenantDbcontext.IsSeeding = true;

        await SeedPermissions.TrySeedAsync(_tenantDbcontext);
        await SeedRoles.TrySeedAsync(_hostDbContext,_tenantDbcontext, tenant?.Id, _logger);
        // seeld localization must be after seed permissions
        await SeedLocalization.TrySeedAsync(_masterDbcontext, _tenantDbcontext, tenant?.Id);
        await SeedStaticMenus.TrySeedAsync(_masterDbcontext, _tenantDbcontext, tenant?.Id);
        await SeedDefaultUsers.TrySeedAsync(_tenantDbcontext, tenant?.Id, _logger, _userManager, _roleManager);
        await SeedSecuritySettings.TrySeedAsync(_tenantDbcontext, tenant?.Id);
        await SeedProductCategoriesFromFile.TrySeedAsync(_tenantDbcontext, tenant?.Id);
        await SeedCurrencies.TrySeedAsync(_tenantDbcontext, tenant?.Id);
        await SeedEnums.TrySeedAsync(_masterDbcontext);
        await SeedExternalProviders.TrySeedAsync(_masterDbcontext);
        await SeedPages.TrySeedAsync(_tenantDbcontext, tenant?.Id);
        await SeedDefaultFolders.TrySeedAsync(_tenantDbcontext, tenant?.Id, _fileManagerSettings);

        _masterDbcontext.IsSeeding = false;
        _tenantDbcontext.IsSeeding = false;

    }



}
