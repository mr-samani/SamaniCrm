using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Domain.Entities;


namespace SamaniCrm.Application.Common.Interfaces;

public interface IIdentityService
{
    //Task<ApplicationUser?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    //Task<ApplicationUser?> FindUserByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken);
    Task<List<(Guid id, string roleName)>> GetAllRolesAsync( CancellationToken cancellationToken);
    Task<List<string>> GetUserRolesByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken);

    Task<UserDTO> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserDTO> GetUserDetailsByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken);

    Task<bool> IsUniqueUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<PaginatedResult<UserDTO>> GetAllUsersAsync(GetUserQuery request, CancellationToken cancellationToken);

    Task<(bool isSucceed, Guid userId)> CreateUserAsync(CreateUserCommand input, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(EditUserCommand input, CancellationToken cancellationToken);
    Task<bool> UpdateUserRoles(string userName, IList<string> usersRole, CancellationToken cancellationToken);
    Task<bool> updateUserLanguage(string culture, Guid userId, CancellationToken cancellationToken);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<(Guid id, string roleName)> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> AssigRolesToUser(string userName, IList<string> roles);
    Task<bool> UpdateRoleAsync(Guid id, string roleName, CancellationToken cancellationToken);
    Task<bool> DeleteRoleAsync(Guid roleId);

    Task<bool> UpdateRolePermissionsAsync(EditRolePermissionsCommand request, CancellationToken cancellationToken);

    Task<LoginResult> LoginAsync(LoginCommand request, CancellationToken cancellationToken);
    Task<LoginResult> TwoFactorLoginAsync(TwoFactorLoginCommand request, CancellationToken cancellationToken);
    Task<LoginResult> ExternalLoginAsync(ExternalLoginCallbackCommand request, CancellationToken cancellationToken);
    Task<LoginResult> DelegateUserAsync(DelegateUserCommand request, CancellationToken cancellationToken);
    Task ExitDelegationAsync(CancellationToken cancellationToken);
    Task LogoutAsync(CancellationToken cancellationToken);

    Task<PaginatedResult<TenantUserDTO>> GetTenantUsersAsync(GetTenantUsersQuery request, CancellationToken cancellationToken);
    Task<List<AutoCompleteDto<Guid>>> GetAutoCompleteUsersAsync(string? filter, CancellationToken cancellationToken);
    Task<List<Guid>> GetAllActiveUsersIdsAsync(CancellationToken cancellationToken);
}
