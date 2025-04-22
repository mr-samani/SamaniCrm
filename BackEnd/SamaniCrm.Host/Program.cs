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
    .AddCustomServices();

var app = builder.Build();

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

app.Run();
