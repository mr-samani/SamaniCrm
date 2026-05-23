using FluentValidation;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.User.Queries;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{


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
}