using SamaniCrm.Application;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Menu.Queries;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Infrastructure.Cache;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Public.Client.Pages;
using SamaniCrm.Public.Components;
using SamaniCrm.Public.Extensions;
using SamaniCrm.Public.Middlewares;
using System.Reflection;

namespace SamaniCrm.Public
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                var services = builder.Services;
                var config = builder.Configuration;
                // Add services to the container.
                services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents();
                services
                    .AddCustomServices()
                    .AddDbContext(config);
                builder.Services.AddCustomMediatR(
                    typeof(GetAllMenuItemsQuery).Assembly,
                    typeof(GetPageInfoQuery).Assembly
                );
                services
                    .AddInfrastructure(config)
                    .AddCacheService(config)
                   ;



                var app = builder.Build();
                await LanguageService.PreloadAllLocalizationsAsync(app.Services);


                // برای اینکه از همان instance ICaptchaStore استفاده کنم و یک نمون جدید نسازد این جا مقدار دهی میکنم
                var captchaStore = app.Services.GetRequiredService<ICaptchaStore>();
                VerifyCaptchaExtensions.Configure(captchaStore, config);




                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseWebAssemblyDebugging();

                    // more log for develop 
                    builder.Logging.AddConsole(); 

                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseAntiforgery();
                app.UseMiddleware<LanguageMiddleware>();

                app.MapStaticAssets();
                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
