using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Persistence;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.DbMigrator;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string not found.");

        // ✅ DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Transient);
        return services;
    }


    public static IServiceCollection AddCustomService(this IServiceCollection services, IConfiguration configuration)
    {
        // Application Services
        services.AddScoped<ICurrentUserService, DummyCurrentUserService>();
        services.AddScoped<ApplicationDbInitializer>();

        //var serviceProvider = services.BuildServiceProvider();
        //var logger = serviceProvider.GetService<ILogger<ApplicationDbInitializer>>();
        //services.AddSingleton(typeof(ILogger), logger!);

        services.AddLogging(logging => logging.AddConsole());
        services.AddScoped<ICurrentUserService, DummyCurrentUserService>();


        return services;
    }


    public class DummyCurrentUserService : ICurrentUserService
    {
        public string? UserId => "MigrationUser"; // یا null هم میتونی بدی
        public string? UserName => "MigrationUser"; // یا null هم میتونی بدی

        public string lang => "fa-IR";

        string ICurrentUserService.lang { get => lang; set => throw new NotImplementedException(); }
    }


}
