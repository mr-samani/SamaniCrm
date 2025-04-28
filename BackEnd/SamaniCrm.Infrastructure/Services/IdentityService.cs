using System.Threading;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Infrastructure.Identity;
using System.Linq.Dynamic.Core;


namespace SamaniCrm.Infrastructure.Services;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
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
        var result = await _roleManager.CreateAsync(new ApplicationRole(roleName));
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
            Email = email,
            Lang = "fa",
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

    public async Task<bool> DeleteRoleAsync(Guid roleId)
    {
        var roleDetails = await _roleManager.FindByIdAsync(roleId.ToString());
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

    public async Task<PaginatedResult<UserResponseDTO>> GetAllUsersAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();
        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x =>
            x.UserName.Contains(request.Filter) ||
            x.FirstName.Contains(request.Filter) ||
            x.LastName.Contains(request.Filter) ||
            x.Email.Contains(request.Filter) ||
            x.PhoneNumber.Contains(request.Filter) 
            );
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortString = $"{request.SortBy} {request.SortDirection}";
            query = query.OrderBy(sortString);
        }

        int total = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Select(u => new UserResponseDTO
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                FirstName = u.FirstName ?? "",
                LastName = u.LastName,
                FullName = u.FullName ?? "",
                Lang = u.Lang ?? "",
                Email = u.Email ?? "",
                ProfilePicture = u.ProfilePicture ?? "",
                Address = u.Address ??"",
                PhoneNumber = u.PhoneNumber ??"",
                CreationTime = u.CreationTime.ToUniversalTime()
            })
            .ToListAsync(cancellationToken);


        return new PaginatedResult<UserResponseDTO>
        {
            Items = users,
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public Task<List<(UserResponseDTO user, IList<string> roles)>> GetAllUsersDetailsAsync()
    {
        throw new NotImplementedException();

        //var roles = await _userManager.GetRolesAsync(user);
        //return (user.Id, user.UserName, user.Email, roles);

        //var users = _userManager.Users.ToListAsync();
    }

    public async Task<List<(Guid id, string roleName)>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();

        return roles.Select(role => (role.Id, role.Name)).ToList();
    }

    public async Task<(UserResponseDTO user, IList<string> roles)> GetUserDetailsAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (new UserResponseDTO()
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            FullName = user.FullName ?? "",
            Lang = user.Lang,
            Email = user.Email ?? "",
            ProfilePicture = user.ProfilePicture ?? "",
            Address = user.Address ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            CreationTime = user.CreationTime.ToUniversalTime()
        }, roles);
    }

    public async Task<(UserResponseDTO user, IList<string> roles)> GetUserDetailsByUserNameAsync(string userName)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (new UserResponseDTO()
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            FullName = user.FullName ?? "",
            Lang = user.Lang,
            Email = user.Email ?? "",
            ProfilePicture = user.ProfilePicture ?? "",
            Address = user.Address ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            CreationTime = user.CreationTime.ToUniversalTime()
        }, roles);
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

    public async Task<(Guid id, string roleName)> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        return (role.Id, role.Name);
    }

    public async Task<bool> UpdateRole(Guid id, string roleName)
    {
        if (roleName != null)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
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
        .FirstOrDefaultAsync(rt => rt.RefreshTokenValue == refreshToken, cancellationToken);

        if (token == null || token.Active == false)
            return false;

        token.Active = false;
        _applicationDbContext.RefreshTokens.Update(token);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }
}
