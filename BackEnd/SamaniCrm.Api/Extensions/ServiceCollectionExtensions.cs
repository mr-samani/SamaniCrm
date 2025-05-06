using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.Email;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SamaniCrm.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{

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
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(GetRoleQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(InitialAppQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetCurrentUserQueryHandler).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }


    /// <summary>
    /// فعال سازی ولیدیشن های ورودی به برنامه با استفاده از FluentValidation
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<GetUserQueryValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
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
    /// اجازه دسترسی به برنامه توسط کلاینت ها
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
            {
                policy.WithOrigins("https://localhost:44342", "http://localhost:5753", "https://localhost:5753")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddControllersWithDefaults(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                //options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                //options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status401Unauthorized));
                // options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status403Forbidden));
            })
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                opt.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverterFactory());
                // opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        return services;
    }


    /// <summary>
    /// Swagger Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SamaniCrm API", Version = "v1" });
            c.AddServer(new OpenApiServer
            {
                Url = "https://localhost:44342",
                Description = "localhost"
            });
            c.AddServer(new OpenApiServer
            {
                Url = "https://api.samani-crm.com",
                Description = "Production Server"
            });
            c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.SchemaFilter<AddEnumNamesSchemaFilter>();

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }


    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
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
        services.AddTransient<IEmailSender<ApplicationUser>, MyEmailSender>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddSingleton(TimeProvider.System);

        services.AddMemoryCache();
        //چون حافظه ایه Singleton باشه بهتره.
        services.AddSingleton<ICaptchaStore, InMemoryCaptchaStore>();
        services.AddHostedService<CaptchaCleanupBackgroundService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();

        services.AddScoped<PermissionFilter>();

        services.AddControllers(options =>
        {
            options.Filters.Add<PermissionFilter>();
        });
        return services;
    }




    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        //services.AddAuthorization(options =>
        //{
        //    foreach (var permission in Enum.GetNames(typeof(Permission)))
        //    {
        //        options.AddPolicy($"Permission:{permission}", policy =>
        //        {
        //            policy.Requirements.Add(new PermissionRequirement(permission));
        //        });
        //    }
        //});

        //services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}


public class AddEnumNamesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsEnum)
        {
            var enumNames = Enum.GetNames(type);
            var enumNamesArray = new OpenApiArray();
            foreach (var name in enumNames)
            {
                enumNamesArray.Add(new OpenApiString(name));
            }

            schema.Extensions.Add("x-enum-varnames", enumNamesArray);
        }
    }
}


public class FlexibleEnumJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(FlexibleEnumJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class FlexibleEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (int.TryParse(str, out var intVal) && Enum.IsDefined(typeof(T), intVal))
                return (T)(object)intVal;

            if (Enum.TryParse<T>(str, ignoreCase: true, out var enumVal))
                return enumVal;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var val))
        {
            if (Enum.IsDefined(typeof(T), val))
                return (T)(object)val;
        }

        throw new JsonException($"Cannot convert value to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

