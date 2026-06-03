using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts;

public class MasterDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "SamaniCrm.Api")
        );

        var appsettingsPath = Path.Combine(basePath, "appsettings.json");

        if (!File.Exists(appsettingsPath))
        {
            throw new FileNotFoundException($"appsettings.json not found at: {appsettingsPath}");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        // برای MasterDbContext، باید از connection string مربوط به Master استفاده کنید.
        var connectionString = configuration.GetConnectionString("DefaultConnection"); // یا نامی مثل "MasterConnectionString"

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Master connection string not found for design time migrations.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();

        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("SamaniCrm.Infrastructure")
        );

        return new MasterDbContext(optionsBuilder.Options, null, null);
    }
}