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
        Task<(bool isSucceed, Guid userId)> CreateUserAsync(string userName, string password, string email, string fullName, List<string> roles);
        Task<bool> SigninUserAsync(string userName, string password);
        Task<string> GetUserIdAsync(string userName);
        Task<(Guid userId, string fullName, string UserName, string email,string profilePicture, IList<string> roles)> GetUserDetailsAsync(Guid userId);
        Task<(Guid userId, string fullName, string UserName, string email, string profilePicture, IList<string> roles)> GetUserDetailsByUserNameAsync(string userName);
        Task<string> GetUserNameAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> IsUniqueUserName(string userName);
        Task<List<(Guid id, string fullName, string userName, string email)>> GetAllUsersAsync();
        Task<List<(string id, string userName, string email, IList<string> roles)>> GetAllUsersDetailsAsync();
        Task<bool> UpdateUserProfile(string id, string fullName, string email, IList<string> roles);

        // Role Section
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleId);
        Task<List<(string id, string roleName)>> GetRolesAsync();
        Task<(string id, string roleName)> GetRoleByIdAsync(string id);
        Task<bool> UpdateRole(string id, string roleName);

        // User's Role section
        Task<bool> IsInRoleAsync(Guid userId, string role);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        Task<bool> AssignUserToRole(string userName, IList<string> roles);
        Task<bool> UpdateUsersRole(string userName, IList<string> usersRole);


        // refresh token
        Task<Guid> GetUserIdFromRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken);
    }
}
