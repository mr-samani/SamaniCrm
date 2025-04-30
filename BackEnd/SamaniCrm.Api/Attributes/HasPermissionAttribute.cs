using Microsoft.AspNetCore.Authorization;

namespace SamaniCrm.Api.Attributes;
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}