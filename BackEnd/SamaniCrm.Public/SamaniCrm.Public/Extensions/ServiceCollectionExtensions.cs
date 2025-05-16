using MediatR;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Infrastructure;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Application.Pages.Queries;

namespace SamaniCrm.Public.Extensions
{
    public static  class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            // ✅ DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }


        /// <summary>
        /// Mediator Configuration
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                // cfg.RegisterServicesFromAssembly(typeof(UserListQuery).Assembly);
               // cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.RegisterServicesFromAssemblies(
                      typeof(GetPageInfoQuery).Assembly ,
                      typeof(GetCurrentUserQueryHandler).Assembly
                  );
            });

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
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();  
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<ISecuritySettingService, SecuritySettingService>();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            //چون حافظه ایه Singleton باشه بهتره.
            services.AddSingleton<ICaptchaStore, InMemoryCaptchaStore>();
            services.AddHostedService<CaptchaCleanupBackgroundService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>(); 
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IPageService, PageService>();
              
            return services;
        }




    }
}
