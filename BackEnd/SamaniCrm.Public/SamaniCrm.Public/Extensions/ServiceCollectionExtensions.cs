using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.Menu.Queries;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.Email;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Infrastructure.Services.Product;
using System.Reflection;

namespace SamaniCrm.Public.Extensions
{
    public static  class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            // ✅ DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient);
            return services;
        }


        /// <summary>
        /// Mediator Configuration
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomMediatR(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentException("حداقل یک اسمبلی باید برای رجیستر کردن هندلرها وارد شود.");

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

            // رجیستر رفتارهای pipeline مثل ValidationBehavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }



        /// <summary>
        /// راه اندازی زیرساخت های برنامه
        /// </summary>
        /// <remarks>
        ///  DBContext,
        ///  IDentity,
        ///  Etc...
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityInfrastructure(config);
            return services;
        }

        /// <summary>
        /// ثبت سرویس های برنامه
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IApplicationDbContext, ApplicationDbContext>();  
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<ISecuritySettingService, SecuritySettingService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();


            services.AddScoped<ILanguageService, LanguageService>();
            services.AddSingleton<LocalizationMemoryCache>();
            services.AddScoped<ILocalizer, CachedStringLocalizer>(); 

            //چون حافظه ایه Singleton باشه بهتره.
            services.AddSingleton<ICaptchaStore, InMemoryCaptchaStore>();
           // services.AddHostedService<CaptchaCleanupBackgroundService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>(); 
            services.AddScoped<IPageService, PageService>();
    


            return services;
        }




    }
}
