using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Api.Attributes;

namespace SamaniCrm.Api.Middlewares
{
    public class PermissionFilter : IAsyncActionFilter
    {
        private readonly IUserPermissionService _permissionService;

        public PermissionFilter(IUserPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.ActionDescriptor;
            var permissionAttributes = context.ActionDescriptor.EndpointMetadata
                .OfType<PermissionAttribute>();

            foreach (var attr in permissionAttributes)
            {
                var hasPermission = await _permissionService.HasPermissionAsync(context.HttpContext.User, attr.Permission);
                if (!hasPermission)
                {
                    context.Result = new ForbidResult(); // یا UnauthorizedResult()
                    return;
                }
            }

            await next();
        }
    }

}
