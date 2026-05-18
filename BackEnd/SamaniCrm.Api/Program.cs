using Hangfire;
using Microsoft.OpenApi;
using SamaniCrm.Api.Extensions;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Api.TUS;
using SamaniCrm.Application;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Host.Middlewares;
using SamaniCrm.Infrastructure.Cache;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Hubs;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Middleware;
using Scalar.AspNetCore;




var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services
    .AddDbContext(config)
    .AddCustomServices(config);
services
    .AddCorsPolicy()
    .AddControllersWithDefaults()
    .AddAutoMapper()
    .AddCustomMediatR()
    .AddFluentValidation()
    .AddInfrastructure(config)
    .AddOpenApiDocumentation()
    .AddHangfire(config)
    .AddCacheService(config)
    .AddFileManagerService(config)
    .AddHangfireJobs(config)
    .LoadExternalProviders(config)
    .AddHelthChecks(config)
    .AddLogging(config);





services.AddSignalR();
// 1. اضافه کردن Cache (اجباری برای Session)
builder.Services.AddDistributedMemoryCache();

// 2. اضافه کردن Session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".SamaniCrm.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();
// برای اینکه از همان instance ICaptchaStore استفاده کنم و یک نمون جدید نسازد این جا مقدار دهی میکنم
var captchaStore = app.Services.GetRequiredService<ICaptchaStore>();
VerifyCaptchaExtensions.Configure(captchaStore, config);

// Correlation ID Middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
    {
        context.Request.Headers["X-Correlation-Id"] = Guid.NewGuid().ToString();
    }
    context.Response.Headers["X-Correlation-Id"] =
        context.Request.Headers["X-Correlation-Id"].ToString();

    await next();
});

app.UseMiddleware<LanguageMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.AddDocument("v1"));
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseStaticFiles();

app.UseSession();

// Security Headers
//TODO: CSP security :commented for scalar
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
//    context.Response.Headers.Append("X-Frame-Options", "DENY");
//    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
//    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
//    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
//    await next();
//});






// آگر این خط کامنت نباشد احراز هویت بر اساس کوکی خواهد بود
// app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();


// Multi-Tenant Middleware Pipeline
app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<TenantSecurityMiddleware>();
// app.UseMiddleware<AuditMiddleware>();



app.MapControllers();
//app.MapGroup("/auth2").MapCustomIdentityApi<ApplicationUser>().WithTags(["Auth2"]);
// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapHub<ProvisioningHub>("/hubs/provisioning");
// app.MapHealthChecks("/health");



await LanguageService.PreloadAllLocalizationsAsync(app.Services);

app.MapHub<NotificationHub>("/hubs/notifications");


using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<FileDirectoryInitializer>();
    await initializer.EnsureBaseDirectoriesAsync();
}
app.InitializeTUS(config);

app.Run();



