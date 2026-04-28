using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Migrator.Manager;
using SixLabors.ImageSharp;

using SamaniCrm.DbMigrator;

var basePath = Path.Combine(Directory.GetCurrentDirectory(), "");
// لود کردن تنظیمات از فایل
var configuration = new ConfigurationBuilder()
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
    .Build();


var builder = Host.CreateDefaultBuilder();


builder.ConfigureLogging(logging =>
   {  // حذف تمام پرووایدرهای پیش‌فرض مثل Console یا Debug
       logging.ClearProviders();

       logging.AddConfiguration(configuration.GetSection("Logging"));
       logging.AddConsole();
       logging.AddDebug();
       logging.AddEventSourceLogger();
   });

var services = builder.ConfigureServices(services =>
{
    services.AddDbContext(configuration)
       .AddCustomService(configuration)
       .AddIdentityForMigrator(configuration);

    // Configuration
    services.AddSingleton(configuration);
});







var app = builder.Build();

try
{

    Console.Clear();
    CommandManager.PrintBanner();

    // 3. اجرای دستورات
    await CommandManager.HandleCommandsAsync(args, app.Services);
}
catch (Exception ex)
{
    // فقط ارورهای واقعی را لاگ کن
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
}