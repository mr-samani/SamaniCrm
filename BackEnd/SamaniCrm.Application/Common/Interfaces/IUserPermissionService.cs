using System.Security.Claims;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IUserPermissionService
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
        Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken);

    }

}
