using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts;


public class TenantDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{

    public TenantDbContext CreateDbContext(string[] args)
    {
        // مسیر پروژه API نسبت به پروژه Infrastructure
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

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }



        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();

        // از connection string پیش‌فرض استفاده کنید
        optionsBuilder.UseSqlServer(connectionString,
            o => o.MigrationsAssembly("SamaniCrm.Infrastructure")
            );

        return new TenantDbContext(optionsBuilder.Options, null, null, null, null);
    }
}