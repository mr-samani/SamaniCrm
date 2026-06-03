using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Persistence;

// فرض بر این است که Log, BaseDbContext, MasterDbContext, TenantDbContext, ApplicationDbInitializer
// و سایر وابستگی‌ها در دسترس هستند.

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

    /// <summary>
    /// به طور همزمان migrations را برای MasterDbContext و TenantDbContext اعمال می‌کند.
    /// </summary>
    public static async Task RunAllMigrationsAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Starting migrations for ALL DbContexts...");

        // ابتدا MasterDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var masterContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            await RunMigrationsAsync(masterContext, "Master");
        }

        // سپس TenantDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await RunMigrationsAsync(tenantContext, "Tenant");
        }

        Log.Success("All migrations applied successfully!");
    }

    /// <summary>
    /// migrations را برای یک DbContext خاص اعمال می‌کند.
    /// </summary>
    /// <param name="context">نمونه DbContext</param>
    /// <param name="contextName">نام نمایشی DbContext (مثلاً "Master" یا "Tenant")</param>
    private static async Task RunMigrationsAsync(DbContext context, string contextName)
    {
        Log.Info($"Starting migrations for {contextName} DbContext...");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var pendingList = pendingMigrations.ToList();

        if (pendingList.Count == 0)
        {
            Log.Success($"{contextName} Database is up to date. No pending migrations.");
            return;
        }

        Log.Info($"Found {pendingList.Count} pending migration(s) for {contextName}:");
        foreach (var migration in pendingList)
        {
            Console.WriteLine($"  - {migration}");
        }
        Console.WriteLine();

        try
        {
            await context.Database.MigrateAsync();
            Log.Success($"Successfully applied {pendingList.Count} migration(s) to {contextName} database!");
        }
        catch (Exception ex)
        {
            Log.Error($"Error applying migrations for {contextName} database: {ex.Message}");
            // ممکن است بخواهید exception را re-throw کنید یا لاگ کنید
            throw;
        }
    }

    /// <summary>
    /// داده‌های اولیه را برای برنامه با استفاده از IServiceProvider اجرا می‌کند.
    /// </summary>
    public static async Task RunSeedingAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Starting data seeding...");

        using var scope = serviceProvider.CreateScope();
        // اطمینان حاصل کنید که ApplicationDbInitializer هر دو DbContext را در صورت نیاز مدیریت می‌کند
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();

        // اگر seeder نیاز به DbContext خاصی دارد، باید آن را از scope دریافت کند
        // مثال: var masterContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        // await seeder.SeedAsync(masterContext);

        // فرض می‌کنیم seeder خودش DbContextها را از IServiceProvider دریافت می‌کند
        await seeder.SeedAsync(null); // اگر null پاس می‌دهید، مطمئن شوید در SeedAsync مدیریت می‌شود.

        Log.Success("Data seeding completed successfully!");
    }

    /// <summary>
    /// پایگاه داده را برای MasterDbContext و TenantDbContext حذف می‌کند.
    /// </summary>
    public static async Task DropAllDatabasesAsync(IServiceProvider serviceProvider)
    {
        Log.Warning("Dropping ALL databases...");

        var confirmed = AskForConfirmation("Are you sure you want to drop BOTH Master and Tenant databases? This will delete ALL data!");
        if (!confirmed)
        {
            Log.Info("Operation cancelled.");
            return;
        }

        // ابتدا MasterDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var masterContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            await DropDatabaseAsync(masterContext, "Master");
        }

        // سپس TenantDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await DropDatabaseAsync(tenantContext, "Tenant");
        }

        Log.Success("All databases dropped successfully!");
    }

    /// <summary>
    /// پایگاه داده را برای یک DbContext خاص حذف می‌کند.
    /// </summary>
    private static async Task DropDatabaseAsync(DbContext context, string contextName)
    {
        Log.Warning($"Dropping {contextName} database...");
        try
        {
            await context.Database.EnsureDeletedAsync();
            Log.Success($"{contextName} database dropped successfully!");
        }
        catch (Exception ex)
        {
            Log.Error($"Error dropping {contextName} database: {ex.Message}");
            // ممکن است بخواهید exception را re-throw کنید یا لاگ کنید
            throw;
        }
    }

    /// <summary>
    /// پایگاه داده را برای MasterDbContext و TenantDbContext بازنشانی می‌کند (حذف، ایجاد، migration، seeding).
    /// </summary>
    public static async Task ResetAllDatabasesAsync(IServiceProvider serviceProvider)
    {
        Log.Warning("Resetting ALL databases (drop -> create -> migrate -> seed)...");

        var confirmed = AskForConfirmation("This will delete ALL data from BOTH Master and Tenant databases. Continue?");
        if (!confirmed)
        {
            Log.Info("Operation cancelled.");
            return;
        }

        // ابتدا MasterDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var masterContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            await DropDatabaseAsync(masterContext, "Master");
            await RunMigrationsAsync(masterContext, "Master");
        }

        // سپس TenantDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await DropDatabaseAsync(tenantContext, "Tenant");
            await RunMigrationsAsync(tenantContext, "Tenant");
        }

        // Seeding فقط یک بار اجرا می‌شود، فرض بر این است که ApplicationDbInitializer
        // اگر لازم باشد، داده‌ها را برای هر دو DbContext مدیریت می‌کند.
        await RunSeedingAsync(serviceProvider);

        Log.Success("All databases reset completed!");
    }

    /// <summary>
    /// اطلاعات پایگاه داده را برای MasterDbContext و TenantDbContext نمایش می‌دهد.
    /// </summary>
    public static async Task ShowAllDatabaseInfoAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Gathering database information for ALL DbContexts...");
        Console.WriteLine();

        // اطلاعات MasterDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var masterContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            await ShowDatabaseInfoAsync(masterContext, "Master");
        }

        Console.WriteLine(); // فاصله بین اطلاعات دو دیتابیس

        // اطلاعات TenantDbContext
        using (var scope = serviceProvider.CreateScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await ShowDatabaseInfoAsync(tenantContext, "Tenant");
        }
    }

    /// <summary>
    /// اطلاعات پایگاه داده را برای یک DbContext خاص نمایش می‌دهد.
    /// </summary>
    private static async Task ShowDatabaseInfoAsync(DbContext context, string contextName)
    {
        Log.Info($"--- Information for {contextName} Database ---");
        var connection = context.Database.GetDbConnection();

        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            var connectionStatus = canConnect ? "Connected" : "Disconnected";
            Log.Info($"Connection Status: {connectionStatus}");

            if (canConnect)
            {
                Log.Info($"Server: {connection.DataSource}");
                Log.Info($"Database: {connection.Database}");
            }
            else
            {
                // اگر کانکت نیست، شاید لازم باشد دلیلش را بیشتر بررسی کنید
                Log.Warning($"Could not connect to {contextName} database. Please check connection string and server status.");
                return; // اگر وصل نیست، اطلاعات بیشتر معنی ندارد
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error getting connection info for {contextName} database: {ex.Message}");
            return;
        }

        // تعداد جداول
        try
        {
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
        }
        catch (Exception ex)
        {
            Log.Warning($"Could not get migration info for {contextName} database: {ex.Message}");
        }

        // تعداد رکوردهای جداول اصلی
        Console.WriteLine();
        Log.Info("Record counts:");

        try
        {
            // فرض می‌کنیم این DbSetها در هر دو DbContext موجود هستند یا حداقل مشابهی دارند
            // اگر نام جداول متفاوت است، باید این بخش را برای هر DbContext جداگانه پیاده‌سازی کنید.
            // مثال: اگر Users در MasterDbContext است و Tenants در TenantDbContext

            if (context is TenantDbContext Ctx) // بررسی نوع DbContext برای دسترسی به DbSetهای خاص
            {
                if (Ctx.Users != null)
                {
                    var userCount = await Ctx.Users.CountAsync();
                    Log.Info($"  Users: {userCount}");
                }
                if (Ctx.ProductCategories != null)
                {
                    var pcatCount = await Ctx.ProductCategories.CountAsync();
                    Log.Info($"  Product Categories: {pcatCount}");
                }
                if (Ctx.Products != null)
                {
                    var pCount = await Ctx.Products.CountAsync();
                    Log.Info($"  Products: {pCount}");
                }
            }
            else if (context is MasterDbContext MCtx)
            {
                if (MCtx.Tenants != null)
                {
                    var tCount = await MCtx.Tenants.CountAsync();
                    Log.Info($"  Tenants: {tCount}");
                }

                // اینجا باید DbSetهای مربوط به TenantDbContext را اضافه کنید
                // مثال:
                // if (tenantCtx.TenantData != null)
                // {
                //     var tenantDataCount = await tenantCtx.TenantData.CountAsync();
                //     Log.Info($"  Tenant Data: {tenantDataCount}");
                // }
                // فعلا یک placeholder اضافه می‌کنیم
                Log.Info("  (Tenant-specific record counts not implemented yet)");
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"Could not get record counts for {contextName} database: {ex.Message}");
        }
    }

    /// <summary>
    /// اسکریپت SQL ایجاد دیتابیس را برای MasterDbContext تولید می‌کند.
    /// </summary>
    /// <remarks>
    /// برای TenantDbContext، نیاز به روشی برای تولید اسکریپت یا اجرای جداگانه Migration ها دارید.
    /// </remarks>
    public static async Task GenerateMasterDBCreateScriptAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Generating SQL script for MasterDbContext create...");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

        if (!await context.Database.CanConnectAsync())
        {
            Log.Error("Cannot connect to Master database. Cannot generate script.");
            return;
        }

        try
        {
            // GenerateCreateScript() کل ساختار دیتابیس را بر اساس مدل EF Core ایجاد می‌کند.
            // اگر بخواهید اسکریپت migration ها را تولید کنید، باید از دستورات CLI استفاده کنید.
            var script = context.Database.GenerateCreateScript();

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "master_database_create_script.sql");
            await File.WriteAllTextAsync(outputPath, script);

            Log.Success($"Master Db Create script generated: {outputPath}");
            Log.Info($"File size: {new FileInfo(outputPath).Length:N0} bytes");
        }
        catch (Exception ex)
        {
            Log.Error($"Error generating Master Db Create script: {ex.Message}");
        }
    }

    // متد مشابه برای TenantDbContext در صورت نیاز
    public static async Task GenerateTenantDBCreateScriptAsync(IServiceProvider serviceProvider)
    {
        Log.Info("Generating SQL script for TenantDbContext create...");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        if (!await context.Database.CanConnectAsync())
        {
            Log.Error("Cannot connect to Tenant database. Cannot generate script.");
            return;
        }

        try
        {
            var script = context.Database.GenerateCreateScript();

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "tenant_database_create_script.sql");
            await File.WriteAllTextAsync(outputPath, script);

            Log.Success($"Tenant Db Create script generated: {outputPath}");
            Log.Info($"File size: {new FileInfo(outputPath).Length:N0} bytes");
        }
        catch (Exception ex)
        {
            Log.Error($"Error generating Tenant Db Create script: {ex.Message}");
        }
    }
}

