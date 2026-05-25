using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Api.Middlewares
{
    public class PermissionFilterMiddleware : IAsyncActionFilter
    {
        private readonly IUserPermissionService _permissionService;

        public PermissionFilterMiddleware(IUserPermissionService permissionService)
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
                    //  context.Result = new ForbidResult(); // یا UnauthorizedResult()
                    throw new AccessDeniedException();
                }
            }

            await next();
        }
    }

}
