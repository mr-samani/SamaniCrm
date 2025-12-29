using Hangfire;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Application;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Host.Middlewares;
using SamaniCrm.Infrastructure.Cache;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Hubs;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Api.TUS;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services
    .AddCustomServices()
    .AddDbContext(config);
services
    .AddCorsPolicy()
    .AddControllersWithDefaults()
    .AddAutoMapper()
    .AddCustomMediatR()
    .AddFluentValidation()
    .AddInfrastructure(config)
    .AddSwaggerDocumentation()
    .AddHangfire(config)
    .AddCacheService(config)
    .AddFileManagerService(config)
    .AddHangfireJobs(config)
    .LoadExternalProviders(config)
    ;

services.AddSignalR();


var app = builder.Build();
// برای اینکه از همان instance ICaptchaStore استفاده کنم و یک نمون جدید نسازد این جا مقدار دهی میکنم
var captchaStore = app.Services.GetRequiredService<ICaptchaStore>();
VerifyCaptchaExtensions.Configure(captchaStore, config);

app.UseMiddleware<LanguageMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseStaticFiles();
// آگر این خط کامنت نباشد احراز هویت بر اساس کوکی خواهد بود
// app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapGroup("/auth2").MapCustomIdentityApi<ApplicationUser>().WithTags(["Auth2"]);
// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");


await LanguageService.PreloadAllLocalizationsAsync(app.Services);

app.MapHub<NotificationHub>("/hubs/notifications");


using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<FileDirectoryInitializer>();
    await initializer.EnsureBaseDirectoriesAsync();
}
app.InitializeTUS(config);

app.Run();
