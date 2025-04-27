

# 🚀 Hangfire چیه؟

- Hangfire یک **فریمورک برای اجرای کارهای پس‌زمینه (Background Jobs)** در دات‌نت هست.
- بدون نیاز به نوشتن BackgroundService یا IHostedService دستی.
- کارها رو توی **صف** میذاره (Queue) و خودش براساس زمان‌بندی یا درخواست اجراشون میکنه.
- اگر سرور ریستارت بشه یا قطع بشه، **کارهای انجام نشده دوباره از سر گرفته میشه**.  
- یک **داشبورد مدیریتی تحت وب** هم میده که ببینی چه Jobهایی در حال اجرا یا منتظرن.

---

# 🛠️ چرا Hangfire خیلی خفنه؟

- داشبورد مدیریتی رایگان داره
- کارها به صورت مطمئن صف‌بندی میشن (حتی اگر اپ کرش کنه)
- پشتیبانی از **Retry خودکار** (اگر یک Job خطا بخوره خودش چند بار دیگه امتحان میکنه)
- پشتیبانی از **Delay**, **Schedule**, **Recurring** (مثلا هر شب ساعت ۲ اجرا کن)
- میشه تو پروژه‌های کوچیک، متوسط، حتی Enterprise استفاده کرد.
- با دیتابیس SQL Server کار میکنه (یا Redis، MongoDB و غیره هم ساپورت میشه).

---

# 🏗️ چطور Hangfire رو راه بندازی؟

## قدم 1: نصب پکیج‌ها

تو پروژه ASP.NET Core (مثلا پروژه SamaniCRM خودت)، این پکیج رو نصب کن:

```bash
dotnet add package Hangfire
dotnet add package Hangfire.AspNetCore
```

یا از NuGet توی Visual Studio اضافه کن.

---

## قدم 2: تنظیمات Startup (`Program.cs`)

بیا Hangfire رو اضافه کن به پروژه:

```csharp
using Hangfire;
using Hangfire.SqlServer; // اگر از SQL Server استفاده میکنی

var builder = WebApplication.CreateBuilder(args);

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

var app = builder.Build();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();
app.Run();
```

🔹 نکته:
- این `/hangfire` آدرس داشبورد مدیریت Hangfire هست (مثلا `https://localhost:5001/hangfire`).

---

## قدم 3: ساخت Job ها

حالا هرجا خواستی یک Job ثبت کنی، مثلا تو کنترلر لاگین:

```csharp
public class AccountController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // بعد از لاگین موفق، مثلا لاگ بفرست به بکاپ سرور:
        BackgroundJob.Enqueue(() => SendLoginNotification(request.Username));

        return Ok();
    }

    public void SendLoginNotification(string username)
    {
        // کد ارسال نوتیفیکیشن یا لاگ
        Console.WriteLine($"User {username} logged in successfully!");
    }
}
```

✅ اینجا `SendLoginNotification` به صورت **پس‌زمینه** و **غیر همزمان** اجرا میشه.

---

# ⏳ انواع Job هایی که Hangfire ساپورت میکنه:

| نوع Job | توضیح |
|:---|:---|
| `Enqueue` | اجرای سریع بعدی |
| `Schedule` | اجرای تاخیری (مثلا ۱۰ دقیقه دیگه) |
| `Recurring` | اجرای تکرارشونده (مثلا هر روز ساعت ۲ شب) |
| `ContinueWith` | اجرای یک Job بعد از اتمام Job دیگر |

مثلا:

```csharp
// اجرای بعد از 1 دقیقه
BackgroundJob.Schedule(() => SendReminderEmail(), TimeSpan.FromMinutes(1));

// اجرای تکراری هر روز ساعت 3 صبح
RecurringJob.AddOrUpdate("my-daily-job", () => SendDailyReport(), Cron.Daily(3));
```

---

# 🔥 داشبورد Hangfire

وقتی سایت رو ران کنی و بری به `/hangfire`، میتونی ببینی:
- چه کارهایی Pending هستن
- چه کارهایی Succeeded شدن
- چه خطاهایی رخ دادن
- Retry یا Delete بزنی
- حتی دستی Job جدید بسازی

---

# 📋 خلاصه گام‌ها:

| قدم | توضیح |
|:---|:---|
| نصب پکیج‌ها | `Hangfire` و `Hangfire.AspNetCore` |
| ثبت Hangfire در DI | `builder.Services.AddHangfire` |
| ران کردن سرور پردازش Job | `builder.Services.AddHangfireServer` |
| اضافه کردن داشبورد | `app.UseHangfireDashboard` |
| ایجاد Job ها | با `BackgroundJob.Enqueue` و غیره |

---

# 📦 پیشنهادهای حرفه‌ای برای پروژه‌ی SamaniCRM:

- برای مدیریت **Captcha Cleanup** بجای BackgroundService، یک Recurring Job بساز با Hangfire.
- برای ارسال ایمیل خوش آمدگویی یا تایید شماره موبایل بعد از ثبت نام، یک Enqueue Job بساز.
- برای Backup دیتابیس یا ارسال گزارش‌های آماری هر شب، از Schedule یا Recurring استفاده کن.
- توی `/hangfire` داشبورد همه چیز رو ببین و مانیتور کن.
- لاگ خطاهای Job ها رو هم فعال کن که اگر Job ای شکست خورد بفهمی.

---
