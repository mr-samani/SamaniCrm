using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Migrator.Log;
using System.Data;

namespace SamaniCrm.DbMigrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.Clear();
        PrintBanner();

        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");

        var services = new ServiceCollection();
        ConfigureServices(services, configuration, connectionString);

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // بررسی آرگومان‌ها
            var command = args.Length > 0 ? args[0].ToLower() : "help";

            switch (command)
            {
                case "migrate":
                    await RunMigrationsAsync(dbContext);
                    break;

                case "seed":
                    await RunSeedingAsync(serviceProvider);
                    break;

                case "drop":
                    await DropDatabaseAsync(dbContext);
                    break;

                case "reset":
                    await ResetDatabaseAsync(dbContext, serviceProvider);
                    break;

                case "help":
                default:
                    PrintHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred: {ex.Message}");
            if (args.Contains("--verbose") || args.Contains("-v"))
            {
                Log.Error(ex.StackTrace!);
            }
        }
    }

    private static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(@"
╔═══════════════════════════════════════════════════╗
║         Samani CRM - Database Migrator            ║
║                   Version 1.0.0                   ║
╚═══════════════════════════════════════════════════╝
");
        Console.ResetColor();
    }

    private static void PrintHelp()
    {
        Console.WriteLine();
        Log.Info("Available Commands:");
        Console.WriteLine();
        Console.WriteLine("  migrate    - Apply all pending migrations");
        Console.WriteLine("  seed       - Seed initial data");
        Console.WriteLine("  drop       - Drop the database");
        Console.WriteLine("  reset      - Drop, recreate, migrate and seed");
        Console.WriteLine("  help       - Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run -- migrate");
        Console.WriteLine("  dotnet run -- seed");
        Console.WriteLine("  dotnet run -- reset");
        Console.WriteLine();
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "");
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
    }

    private static void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Application Services
        //  services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<ICurrentUserService, DummyCurrentUserService>();
        services.AddScoped<ApplicationDbInitializer>();

        // Configuration
        services.AddSingleton(configuration);
    }

    private static async Task RunMigrationsAsync(ApplicationDbContext context)
    {
        Log.Info("Starting migrations...");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var pendingList = pendingMigrations.ToList();

        if (pendingList.Count == 0)
        {
            Log.Success("Database is up to date. No pending migrations.");
            return;
        }

        Log.Info($"Found {pendingList.Count} pending migration(s):");
        foreach (var migration in pendingList)
        {
            Console.WriteLine($"  - {migration}");
        }
        Console.WriteLine();

        await context.Database.MigrateAsync();

        Log.Success($"Successfully applied {pendingList.Count} migration(s)!");
    }

    private static async Task RunSeedingAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Starting data seeding...");

        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();

        await seeder.SeedAsync();

        Log.Success("Data seeding completed successfully!");
    }

    private static async Task DropDatabaseAsync(ApplicationDbContext context)
    {
        Log.Warning("Dropping database...");

        var confirmed = AskForConfirmation("Are you sure you want to drop the database?");
        if (!confirmed)
        {
            Log.Info("Operation cancelled.");
            return;
        }

        await context.Database.EnsureDeletedAsync();
        Log.Success("Database dropped successfully!");
    }

    private static async Task ResetDatabaseAsync(
        ApplicationDbContext context,
        IServiceProvider serviceProvider)
    {
        Log.Warning("Resetting database (drop -> create -> migrate -> seed)...");

        var confirmed = AskForConfirmation("This will delete ALL data. Continue?");
        if (!confirmed)
        {
            Log.Info("Operation cancelled.");
            return;
        }

        await DropDatabaseAsync(context);
        await RunMigrationsAsync(context);
        await RunSeedingAsync(serviceProvider);

        Log.Success("Database reset completed!");
    }

    private static bool AskForConfirmation(string message)
    {
        Console.WriteLine();
        Console.Write($"{message} (y/N): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }
}