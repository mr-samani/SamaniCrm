using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Middleware;

public class AddCorrelationIdToRequests
{
    private readonly RequestDelegate _next;

    public AddCorrelationIdToRequests(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
        {
            context.Request.Headers["X-Correlation-Id"] = Guid.NewGuid().ToString();
        }
        context.Response.Headers["X-Correlation-Id"] =
            context.Request.Headers["X-Correlation-Id"].ToString();

        await _next(context);
    }
}
