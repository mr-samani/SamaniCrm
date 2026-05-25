using Microsoft.AspNetCore.Http;
using SamaniCrm.Core;
using System.Globalization;

namespace SamaniCrm.Host.Middlewares;

public class LanguageMiddleware
{
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        var lang = context.Request.Headers["lang"].ToString() ??
            context.Request.Cookies["lang"] ??
            AppConsts.DefaultLanguage;

        context.Items["lang"] = lang;
        var culture = new CultureInfo(lang);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        await _next(context);
    }
}

