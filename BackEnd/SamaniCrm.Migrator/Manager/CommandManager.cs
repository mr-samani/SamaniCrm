using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Infrastructure;
using SamaniCrm.Migrator;

namespace SamaniCrm.Migrator.Manager;

public static class CommandManager
{


    /// <summary>
    /// دریافت دستورات کنسول و اجرای فرایندهای مایگریتور
    /// 
    /// <example>
    ///     <code>
    ///         dotnet run                    # Interactive mode"
    ///         dotnet run -- 1               # Run migrate"
    ///         dotnet run -- migrate         # Run migrate"
    ///         dotnet run -- 4 --verbose     # Reset with details"
    ///     </code>
    /// </example>
    /// </summary>
    /// <param name="args">
    /// آرگومان های ورودی
    /// </param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task HandleCommandsAsync(string[] args, IServiceProvider serviceProvider)
    {
        bool continueLoop = true;
        // حلقه اصلی برای اجرای مداوم
        while (continueLoop)
        {
            // ۱. دریافت دستور (از آرگومان یا منوی تعاملی)
            // نکته: اگر آرگومان داده شده، فقط یکبار اجرا کن و تمام.
            // اگر آرگومان نداده، منو رو نشون بده و بچرخ.
            string command;
            if (args.Length > 0)
            {
                var arg = args[0].ToLower();
                if (int.TryParse(arg, out int number))
                {
                    command = CommandManager.NumberToCommand(number) ?? "help";
                }
                else
                {
                    command = arg;
                }
                // اگر آرگومان بود، حلقه رو یکبار اجرا کن و تمام
                continueLoop = false;
            }
            else
            {
                // اگر آرگومان نبود، از کاربر بگیر
                command = CommandManager.GetInteractiveCommand();

                // اگر کاربر خروج زده، حلقه تمام میشه
                if (command == "exit")
                {
                    continueLoop = false;
                    Log.Info("Goodbye!");
                    break;
                }
            }

            // ۲. اجرای دستور
            try
            {
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

                switch (command)
                {
                    case "migrate":
                        await MigratorManager.RunMigrationsAsync(dbContext);
                        break;
                    case "seed":
                        await MigratorManager.RunSeedingAsync(serviceProvider);
                        break;
                    case "drop":
                        await MigratorManager.DropDatabaseAsync(dbContext);
                        break;
                    case "reset":
                        await MigratorManager.ResetDatabaseAsync(dbContext, serviceProvider);
                        break;
                    case "info":
                        await MigratorManager.ShowDatabaseInfoAsync(dbContext);
                        break;
                    case "script":
                        await MigratorManager.GenerateScriptAsync(dbContext);
                        break;
                    case "help":
                        CommandManager.PrintHelp();
                        break;
                    default:
                        Log.Warning("Unknown command. Type 'help' for list.");
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

            // ۳. پرسیدن برای ادامه (فقط اگر حالت تعاملی بود)
            if (!args.Any())
            {
                Console.WriteLine();
                Log.Info("Do you want to perform another operation? (Y/N)");
                var response = Console.ReadLine()?.Trim().ToLower();

                if (response != "y" && response != "yes")
                {
                    continueLoop = false;
                    Log.Info("Exiting application...");
                }
                else
                {
                    Console.Clear(); // پاک کردن صفحه برای منوی بعدی
                }
            }
        }

    }




    public static void PrintBanner()
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

}