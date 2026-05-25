using AutoMapper.Internal;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;
using System.Reflection;
using System.Text.RegularExpressions;



namespace SamaniCrm.Api.Extensions;

public class OperationSchmaTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // اگر قبلاً operationId تنظیم شده، دست نزن
        if (!string.IsNullOrWhiteSpace(operation.OperationId))
            return Task.CompletedTask;

        MethodInfo? methodInfo = context.Description.ActionDescriptor?.EndpointMetadata
            .OfType<MethodInfo>()
            .FirstOrDefault();

        // اگر MethodInfo از metadata پیدا نشد، از نام endpoint استفاده کن
        if (methodInfo == null)
        {

            var fallback = context.Description.ActionDescriptor?.RouteValues["action"]
                           ?? context.Description.RelativePath
                           ?? "operation";

            operation.OperationId = SanitizeOperationId(fallback);
            return Task.CompletedTask;
        }

        var controllerName = methodInfo.DeclaringType?.Name?.Replace("Controller", "");
        var methodName = methodInfo.Name;

        if (string.IsNullOrWhiteSpace(controllerName))
        {
            var fallback = context.Description.RelativePath
                           ?.Split('/')
                           .LastOrDefault()?
                           .Replace("{", "")
                           .Replace("}", "")
                           ?? methodName;

            operation.OperationId = SanitizeOperationId(fallback);
        }
        else
        {
            operation.OperationId = SanitizeOperationId($"{methodName}");
        }

        return Task.CompletedTask;
    }
    private static string SanitizeOperationId(string value)
    {
        return Regex.Replace(value, @"[^a-zA-Z0-9_]", "");
    }

}











