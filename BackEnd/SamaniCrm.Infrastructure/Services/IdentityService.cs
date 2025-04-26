using System.Threading;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure.Identity;



namespace SamaniCrm.Infrastructure.Services;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _roleManager = roleManager;
    }

    public async Task<bool> AssignUserToRole(string userName, IList<string> roles)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.AddToRolesAsync(user, roles);
        return result.Succeeded;
    }

    public async Task<bool> CreateRoleAsync(string roleName)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors);
        }
        return result.Succeeded;
    }


    // Return multiple value
    public async Task<(bool isSucceed, Guid userId)> CreateUserAsync(string userName, string password, string email, string fullName, List<string> roles)
    {
        var user = new ApplicationUser()
        {
            FullName = fullName,
            UserName = userName,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors);
        }

        var addUserRole = await _userManager.AddToRolesAsync(user, roles);
        if (!addUserRole.Succeeded)
        {
            throw new ValidationException(addUserRole.Errors);
        }
        return (result.Succeeded, user.Id);
    }

    public async Task<bool> DeleteRoleAsync(string roleId)
    {
        var roleDetails = await _roleManager.FindByIdAsync(roleId);
        if (roleDetails == null)
        {
            throw new NotFoundException("Role not found");
        }

        if (roleDetails.Name == "Administrator")
        {
            throw new BadRequestException("You can not delete Administrator Role");
        }
        var result = await _roleManager.DeleteAsync(roleDetails);
        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors);
        }
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
            //throw new Exception("User not found");
        }

        if (user.UserName == "system" || user.UserName == "admin")
        {
            throw new Exception("You can not delete system or admin user");
            //throw new BadRequestException("You can not delete system or admin user");
        }
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<List<(Guid id, string fullName, string userName, string email)>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.Select(x => new
        {
            x.Id,
            x.FullName,
            x.UserName,
            x.Email
        }).ToListAsync();

        return users.Select(user => (user.Id, user.FullName, user.UserName, user.Email)).ToList();
    }

    public Task<List<(string id, string userName, string email, IList<string> roles)>> GetAllUsersDetailsAsync()
    {
        throw new NotImplementedException();

        //var roles = await _userManager.GetRolesAsync(user);
        //return (user.Id, user.UserName, user.Email, roles);

        //var users = _userManager.Users.ToListAsync();
    }

    public async Task<List<(string id, string roleName)>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();

        return roles.Select(role => (role.Id, role.Name)).ToList();
    }

    public async Task<(Guid userId, string fullName, string UserName, string email, string profilePicture, IList<string> roles)> GetUserDetailsAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (user.Id, user.FullName, user.UserName, user.Email, user.ProfilePicture, roles);
    }

    public async Task<(Guid userId, string fullName, string UserName, string email, string profilePicture, IList<string> roles)> GetUserDetailsByUserNameAsync(string userName)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (user.Id, user.FullName, user.UserName, user.Email,user.ProfilePicture, roles);
    }

    public async Task<string> GetUserIdAsync(string userName)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new NotFoundException("User not found");
            //throw new Exception("User not found");
        }
        return await _userManager.GetUserIdAsync(user);
    }

    public async Task<string> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
            //throw new Exception("User not found");
        }
        return await _userManager.GetUserNameAsync(user);
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> IsUniqueUserName(string userName)
    {
        return await _userManager.FindByNameAsync(userName) == null;
    }

    public async Task<bool> SigninUserAsync(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);
        return result.Succeeded;


    }

    public async Task<bool> UpdateUserProfile(string id, string fullName, string email, IList<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        user.FullName = fullName;
        user.Email = email;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<(string id, string roleName)> GetRoleByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        return (role.Id, role.Name);
    }

    public async Task<bool> UpdateRole(string id, string roleName)
    {
        if (roleName != null)
        {
            var role = await _roleManager.FindByIdAsync(id);
            role.Name = roleName;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
        return false;
    }

    public async Task<bool> UpdateUsersRole(string userName, IList<string> usersRole)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var existingRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        result = await _userManager.AddToRolesAsync(user, usersRole);

        return result.Succeeded;
    }

    public async Task<Guid> GetUserIdFromRefreshToken(string refreshToken)
    {
        var refreshTokenResult = await _applicationDbContext.RefreshTokens.FirstOrDefaultAsync(q => q.RefreshTokenValue == refreshToken);
        if (refreshTokenResult is null ||
                       refreshTokenResult.Active == false ||
                       refreshTokenResult.Expiration <= DateTime.UtcNow)
        {
            return Guid.Empty;
        }

        if (refreshTokenResult.Used)
        {
            // _logger.LogWarning("El refresh token del {UserId} ya fue usado. RT={RefreshToken}", refreshToken.UserId, refreshToken.RefreshTokenValue);
            var refreshTokens = await _applicationDbContext.RefreshTokens
                .Where(q => q.Active && q.Used == false && q.UserId == refreshTokenResult.UserId)
                .ToListAsync();

            foreach (var rt in refreshTokens)
            {
                rt.Used = true;
                rt.Active = false;
            }

            await _applicationDbContext.SaveChangesAsync();

            return Guid.Empty;
        }


        refreshTokenResult.Used = true;

        var user = await _applicationDbContext.Users.FindAsync(refreshTokenResult.UserId);

        if (user is null)
        {
            return Guid.Empty;
        }

        return user.Id;
    }

    public async Task<bool> RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _applicationDbContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.AccessToken == refreshToken, cancellationToken);

        if (token == null || token.Active == false)
            return false;

        token.Active = false;
        _applicationDbContext.RefreshTokens.Update(token);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }
}
