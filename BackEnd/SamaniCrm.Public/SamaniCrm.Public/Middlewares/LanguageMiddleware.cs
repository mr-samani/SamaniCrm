using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core;
using System.Globalization;

namespace SamaniCrm.Public.Middlewares;
public class LanguageMiddleware
{
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;

        // فقط اجازه بده برای page-level requests اجرا بشه
        if (!path.StartsWithSegments("/_blazor") &&
            !path.StartsWithSegments("/_framework") &&
            !path.StartsWithSegments("/favicon") &&
            !path.StartsWithSegments("/css") &&
            !path.StartsWithSegments("/js") &&
            !path.StartsWithSegments("/lib"))
        {
            var lang = context.Request.Headers["lang"].ToString() ??
            context.Request.Cookies["lang"] ??
            AppConsts.DefaultLanguage;
            lang = lang == string.Empty ? AppConsts.DefaultLanguage : lang;

            context.Items["lang"] = lang;
            var culture = new CultureInfo(lang);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        await _next(context);
    }
}

