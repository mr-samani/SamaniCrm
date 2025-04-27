using Hangfire;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application;
using SamaniCrm.Host.Middlewares;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services
    .AddCorsPolicy()
    .AddControllersWithDefaults()
    .AddCustomMediatR()
    .AddFluentValidation()
    .AddJwtAuthentication(config)
    .AddInfrastructure(config)
    .AddSwaggerDocumentation()
    .AddHangfire(config)
    .AddCustomServices();

var app = builder.Build();
// برای اینکه از همان instance ICaptchaStore استفاده کنم و یک نمون جدید نسازد این جا مقدار دهی میکنم
var captchaStore = app.Services.GetRequiredService<ICaptchaStore>();
VerifyCaptchaExtensions.Configure(captchaStore);


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();
//app.MapGroup("/auth2").MapCustomIdentityApi<ApplicationUser>().WithTags(["Auth2"]);
// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");


app.Run();
