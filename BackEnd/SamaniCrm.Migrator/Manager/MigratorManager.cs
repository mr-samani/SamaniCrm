using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Persistence;

namespace SamaniCrm.Migrator.Manager;

public static class MigratorManager
{ 
    private static bool AskForConfirmation(string message)
    {
        Console.WriteLine();
        Console.Write($"{message} (y/N): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    public static async Task RunMigrationsAsync(ApplicationDbContext context)
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

    public static async Task RunSeedingAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Starting data seeding...");

        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();

        await seeder.SeedAsync();

        Log.Success("Data seeding completed successfully!");
    }

    public static async Task DropDatabaseAsync(ApplicationDbContext context)
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

    public static async Task ResetDatabaseAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
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




    public static async Task ShowDatabaseInfoAsync(ApplicationDbContext context)
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
            var userCount = await context.Users.CountAsync();
            Log.Info($"  Users: {userCount}");
            
            var pcatCount = await context.ProductCategories.CountAsync();
            Log.Info($"  Product Categories: {pcatCount}");

            var pCount = await context.Products.CountAsync();
            Log.Info($"  Products: {pcatCount}");

        }
        catch (Exception ex)
        {
            Log.Warning($"Could not get record counts: {ex.Message}");
        }
    }

    public static async Task GenerateScriptAsync(ApplicationDbContext context)
    {
        Log.Info("Generating SQL migration script...");

        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "migration_script.sql");

        var script = context.Database.GenerateCreateScript();

        await File.WriteAllTextAsync(outputPath, script);

        Log.Success($"Script generated: {outputPath}");
        Log.Info($"File size: {new FileInfo(outputPath).Length:N0} bytes");
    }
}