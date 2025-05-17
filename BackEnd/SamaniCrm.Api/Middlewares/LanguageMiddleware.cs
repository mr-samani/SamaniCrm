using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core;

namespace SamaniCrm.Api.Middlewares;
public class LanguageMiddleware
{
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsunc(HttpContext httpContext, ICurrentUserService currentUserService)
    {
        var lang = httpContext.Request.Headers["lang"].ToString();
        if (!string.IsNullOrEmpty(lang) && currentUserService.lang != "")
        {
            lang = currentUserService.lang;
        }
        lang ??= AppConsts.DefaultLanguage;
        httpContext.Items["lang"] = lang;

        await _next(httpContext);
    }
}

