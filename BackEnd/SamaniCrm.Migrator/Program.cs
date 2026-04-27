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

        // دریافت دستور از آرگومان یا از کاربر
        var command = GetCommand(args);

        try
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

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
                case "info":
                    await ShowDatabaseInfoAsync(dbContext);
                    break;
                case "script":
                    await GenerateScriptAsync(dbContext);
                    break;
                case "exit":
                    Log.Info("Goodbye!");
                    break;
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
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    Available Commands                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("  Usage: dotnet run -- [command|number] [options]");
        Console.WriteLine();
        Console.WriteLine("  Commands:");
        Console.WriteLine("  ┌──────────┬──────────────────────────────────────────┐");
        Console.WriteLine("  │ Number   │ Description                              │");
        Console.WriteLine("  ├──────────┼──────────────────────────────────────────┤");
        Console.WriteLine("  │ 1/migrate│ Apply all pending migrations             │");
        Console.WriteLine("  │ 2/seed   │ Seed initial data                        │");
        Console.WriteLine("  │ 3/drop   │ Drop the database                        │");
        Console.WriteLine("  │ 4/reset  │ Drop + Migrate + Seed                    │");
        Console.WriteLine("  │ 5/info   │ Show database information                │");
        Console.WriteLine("  │ 6/script │ Generate SQL migration script            │");
        Console.WriteLine("  │ 7/help   │ Show this help message                   │");
        Console.WriteLine("  │ 0/exit   │ Exit application                         │");
        Console.WriteLine("  └──────────┴──────────────────────────────────────────┘");
        Console.WriteLine();
        Console.WriteLine("  Options:");
        Console.WriteLine("    --verbose, -v    Show detailed error information");
        Console.WriteLine();
        Console.WriteLine("  Examples:");
        Console.WriteLine("    dotnet run                    # Interactive mode");
        Console.WriteLine("    dotnet run -- 1               # Run migrate");
        Console.WriteLine("    dotnet run -- migrate         # Run migrate");
        Console.WriteLine("    dotnet run -- 4 --verbose     # Reset with details");
        Console.WriteLine();
    }
    private static string GetCommand(string[] args)
    {
        // اگه آرگومان خط فرمان داده شده، استفاده کن
        if (args.Length > 0)
        {
            var arg = args[0].ToLower();

            // اگه عدد وارد شده، تبدیل به دستور کن
            if (int.TryParse(arg, out int number))
            {
                return NumberToCommand(number);
            }

            // اگه خود دستور وارد شده
            return arg;
        }

        // وگرنه از کاربر بگیر
        return GetInteractiveCommand();
    }

    private static string GetInteractiveCommand()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Database Management Menu                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  ┌─────────────────────────────────────────────────────┐");
            Console.WriteLine("  │  [1]  Migrate      - Apply pending migrations       │");
            Console.WriteLine("  │  [2]  Seed         - Seed initial data              │");
            Console.WriteLine("  │  [3]  Drop         - Drop the database              │");
            Console.WriteLine("  │  [4]  Reset        - Drop + Migrate + Seed          │");
            Console.WriteLine("  │  [5]  Info         - Show database information      │");
            Console.WriteLine("  │  [6]  Script       - Generate SQL migration script  │");
            Console.WriteLine("  │  [7]  Help         - Show all commands              │");
            Console.WriteLine("  │  [0]  Exit         - Exit application               │");
            Console.WriteLine("  └─────────────────────────────────────────────────────┘");
            Console.WriteLine();

            Console.Write("  Press a key [0-7] or type command: ");

            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Log.Warning("Please enter a valid option.");
                continue;
            }

            // اگه عدد وارد شده
            if (int.TryParse(input, out int number))
            {
                if (number == 0)
                {
                    return "exit";
                }

                var command = NumberToCommand(number);
                if (command != null)
                {
                    return command;
                }

                Log.Warning($"Invalid option: {number}. Please choose between 0-7.");
                continue;
            }

            // اگه دستور مستقیم وارد شده
            var validCommands = new[] { "migrate", "seed", "drop", "reset", "info", "script", "help", "exit" };
            if (validCommands.Contains(input.ToLower()))
            {
                return input.ToLower();
            }

            Log.Warning($"Unknown command: {input}");
            Console.WriteLine();
            Log.Info("Type a number [0-7] or command name.");
        }
    }

    private static string? NumberToCommand(int number)
    {
        return number switch
        {
            1 => "migrate",
            2 => "seed",
            3 => "drop",
            4 => "reset",
            5 => "info",
            6 => "script",
            7 => "help",
            0 => "exit",
            _ => null
        };
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


    private static async Task ShowDatabaseInfoAsync(ApplicationDbContext context)
    {
        Log.Info("Gathering database information...");
        Console.WriteLine();

        // اطلاعات سرور و دیتابیس
        var serverVersion = context.Database.CanConnect() ? "Connected" : "Disconnected";
        Log.Info($"Connection Status: {serverVersion}");

        if (context.Database.IsSqlServer())
        {
            var connection = context.Database.GetDbConnection();
            Log.Info($"Server: {connection.DataSource}");
            Log.Info($"Database: {connection.Database}");
        }

        // تعداد جداول
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

        Console.WriteLine();
        Log.Info($"Pending Migrations: {pendingMigrations.Count()}");
        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine($"  → {migration}");
        }

        Console.WriteLine();
        Log.Info($"Applied Migrations: {appliedMigrations.Count()}");
        foreach (var migration in appliedMigrations)
        {
            Console.WriteLine($"  ✓ {migration}");
        }

        // تعداد رکوردهای جداول اصلی
        Console.WriteLine();
        Log.Info("Record counts:");

        try
        {
            // اینجا اسم جداول خودت رو بذار
            // var userCount = await context.Users.CountAsync();
            // Log.Info($"  Users: {userCount}");
        }
        catch (Exception ex)
        {
            Log.Warning($"Could not get record counts: {ex.Message}");
        }
    }

    private static async Task GenerateScriptAsync(ApplicationDbContext context)
    {
        Log.Info("Generating SQL migration script...");

        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "migration_script.sql");

        var script = context.Database.GenerateCreateScript();

        await File.WriteAllTextAsync(outputPath, script);

        Log.Success($"Script generated: {outputPath}");
        Log.Info($"File size: {new FileInfo(outputPath).Length:N0} bytes");
    }

}