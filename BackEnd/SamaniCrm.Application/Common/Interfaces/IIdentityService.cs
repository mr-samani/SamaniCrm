using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        // User section
        Task<(bool isSucceed, Guid userId)> CreateUserAsync(CreateUserCommand input);
        Task<string> GetUserIdAsync(string userName);
        Task<UserDTO> GetUserDetailsAsync(Guid userId);
        Task<UserDTO> GetUserDetailsByUserNameAsync(string userName);
        Task<string> GetUserNameAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> IsUniqueUserName(string userName);
        Task<PaginatedResult<UserDTO>> GetAllUsersAsync(GetUserQuery request, CancellationToken cancellationToken);
        Task<bool> UpdateUser(EditUserCommand input);

        // Role Section
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(Guid roleId);
        Task<List<(Guid id, string roleName)>> GetRolesAsync();
        Task<(Guid id, string roleName)> GetRoleByIdAsync(Guid id);
        Task<bool> UpdateRole(Guid id, string roleName);

        // User's Role section
        Task<bool> IsInRoleAsync(Guid userId, string role);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<bool> AssignUserToRole(string userName, IList<string> roles);
        Task<bool> UpdateUsersRole(string userName, IList<string> usersRole);


        // refresh token
        Task<Guid> GetUserIdFromRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<bool> UpdateRolePermissionsAsync(EditRolePermissionsCommand request, CancellationToken cancellationToken);

        Task<bool> updateUserLanguage(string culture, Guid userId, CancellationToken cancellationToken);
        Task<(bool EnableTwoFactor, string Secret, int AttemptCount, TwoFactorTypeEnum TwoFactorType)> getUserTwoFactorData(Guid userId, CancellationToken cancellationToken);
        Task<LoginResult> SignInAsync(LoginCommand request, CancellationToken cancellationToken);
        Task<LoginResult> TwofactorSignInAsync(TwoFactorLoginCommand request, CancellationToken cancellationToken);
        Task<LoginResult> ExternalSignInAsync(ExternalLoginCallbackCommand request, CancellationToken cancellationToken);



    }
}
