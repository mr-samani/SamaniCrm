using AutoMapper.Internal;
using Azure.Core;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.ExternalLogin;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Notifications;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using static QRCoder.PayloadGenerator;
using UnauthorizedAccessException = SamaniCrm.Application.Common.Exceptions.UnauthorizedAccessException;


namespace SamaniCrm.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserPermissionService _userPermissionService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly HttpClient _httpClient;
    private readonly ISecretStore _secretStore;
    private readonly IConfiguration _config;
    private readonly IExternalLoginService _externalLoginService;
    private readonly ICurrentUserService _currentUser;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext applicationDbContext,
        IHttpContextAccessor httpContextAccessor,
        IUserPermissionService userPermissionService,
        ISecuritySettingService securitySettingService,
        ITwoFactorService twoFactorService,
        HttpClient httpClient,
        ISecretStore secretStore,
        IConfiguration config,
        IExternalLoginService externalLoginService,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _dbContext = applicationDbContext;
        _httpContextAccessor = httpContextAccessor;
        _userPermissionService = userPermissionService;
        _securitySettingService = securitySettingService;
        _twoFactorService = twoFactorService;
        _httpClient = httpClient;
        _secretStore = secretStore;
        _config = config;
        _externalLoginService = externalLoginService;
        _currentUser = currentUser;
    }



    public async Task<SimpleTenantData?> GetTenantByTenancyName(string tenancyName, CancellationToken cancellation)
    {
        var tenant = await _dbContext.Tenants
            .IgnoreQueryFilters()
            .Where(x => x.Slug == tenancyName && x.Status == TenantStatus.Active)
            .Select(s => new SimpleTenantData()
            {
                Id = s.Id,
                TenancyName = s.Slug,
                TenantName = s.Name,
                Status = s.Status
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellation);
        return tenant;
    }
    public async Task<SimpleTenantData?> GetTenantById(Guid tenantId, CancellationToken cancellation)
    {
        var tenant = await _dbContext.Tenants
             .IgnoreQueryFilters()
             .Where(x => x.Id == tenantId && x.Status == TenantStatus.Active)
             .Select(s => new SimpleTenantData()
             {
                 Id = s.Id,
                 TenancyName = s.Slug,
                 TenantName = s.Name,
                 Status = s.Status
             })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellation);
        return tenant;
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
            throw new CustomValidationException(result.Errors);
        }
        return result.Succeeded;
    }


    // Return multiple value
    public async Task<(bool isSucceed, Guid userId)> CreateUserAsync(CreateUserCommand input)
    {
        var user = new ApplicationUser()
        {
            TenantId = input.TenantId,
            FirstName = input.FirstName,
            LastName = input.LastName,
            FullName = input.FirstName + " " + input.LastName,
            UserName = input.UserName,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            Lang = input.Lang,
            Address = input.Address,
        };

        var result = await _userManager.CreateAsync(user, input.Password);

        if (!result.Succeeded)
        {
            throw new CustomValidationException(result.Errors);
        }

        var addUserRole = await _userManager.AddToRolesAsync(user, input.Roles);
        if (!addUserRole.Succeeded)
        {
            throw new CustomValidationException(addUserRole.Errors);
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
            throw new CustomValidationException(result.Errors);
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

    public async Task<PaginatedResult<UserDTO>> GetAllUsersAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        var rolesQuery = from ur in _dbContext.UserRoles
                         join r in _dbContext.Roles on ur.RoleId equals r.Id
                         select new { ur.UserId, RoleName = r.Name };

        IQueryable<ApplicationUser> query = _dbContext.Users.AsQueryable();
        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x =>
            x.UserName!.Contains(request.Filter) ||
            x.FirstName!.Contains(request.Filter) ||
            x.LastName.Contains(request.Filter) ||
            x.Email!.Contains(request.Filter) ||
            x.PhoneNumber!.Contains(request.Filter)
            );
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortString = $"{request.SortBy} {request.SortDirection}";
            query = query.OrderBy(sortString);
        }

        int total = await query.CountAsync(cancellationToken);

        var users = await query.OrderBy(x => x.CreatedAt)
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                FirstName = u.FirstName ?? "",
                LastName = u.LastName,
                FullName = u.FullName ?? "",
                Lang = u.Lang ?? "",
                Email = u.Email ?? "",
                ProfilePicture = u.ProfilePicture ?? "",
                Address = u.Address ?? "",
                PhoneNumber = u.PhoneNumber ?? "",
                CreationTime = u.CreatedAt,
                Roles = rolesQuery.Where(x => x.UserId == u.Id).Select(x => x.RoleName).ToList()
            })
            .ToListAsync(cancellationToken);


        return new PaginatedResult<UserDTO>
        {
            Items = users,
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PaginatedResult<TenantUserDTO>> GetTenantUsersAsync(GetTenantUsersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<ApplicationUser> query = _dbContext.Users
            .Include(c => c.Roles)
            .Where(x => x.IsDeleted == false && x.TenantId == request.TenantId)
            .IgnoreQueryFilters()
            .AsQueryable();
        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x =>
            x.UserName!.Contains(request.Filter) ||
            x.FirstName!.Contains(request.Filter) ||
            x.LastName.Contains(request.Filter) ||
            x.Email!.Contains(request.Filter) ||
            x.PhoneNumber!.Contains(request.Filter)
            );
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortString = $"{request.SortBy} {request.SortDirection}";
            query = query.OrderBy(sortString);
        }

        int total = await query.CountAsync(cancellationToken);

        var users = await query.OrderBy(x => x.CreatedAt)
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .Select(u => new TenantUserDTO
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                FullName = u.FullName ?? "",
                Email = u.Email ?? "",
                Roles = u.Roles.Select(s => s.Name).ToList()
            })
            .ToListAsync(cancellationToken);


        return new PaginatedResult<TenantUserDTO>
        {
            Items = users,
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }


    public async Task<List<(Guid id, string roleName)>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();

        return roles.Select(role => (role.Id, role.Name!)).ToList();
    }

    public async Task<UserDTO> GetUserDetailsAsync(Guid userId)
    {
        var query = _userManager.Users.Where(x => x.Id == userId);
        // var sql = query.ToQueryString();
        var user = await query.FirstOrDefaultAsync();
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (new UserDTO()
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
            CreationTime = user.CreatedAt,
            Roles = roles.ToList(),

            IsDelegated = _currentUser.IsDelegated,
            DelegatorId = _currentUser.DelegatorId
        });
    }

    public async Task<UserDTO> GetUserDetailsByUserNameAsync(string userName, Guid? tenantId)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserName == userName && x.TenantId == tenantId && !x.IsDeleted);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return (new UserDTO()
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
            CreationTime = user.CreatedAt,
            Roles = roles.ToList()
        });
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
        return await _userManager.GetUserNameAsync(user) ?? "";
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

    private async Task<bool> SigninUserAsync(string userName, string password, Guid? tenantId)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserName == userName && x.TenantId == tenantId && !x.IsDeleted);
        if (user == null)
        {
            return false;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
            return false;

        // await _signInManager.SignInAsync(user, isPersistent: true);
        // وقتی از کوکی استفااده می کنیم باید دستی کلیم ها ای اضافی را ست کنیم - فقط idenity claim های پیش فرض را دارد 
        // وقتی از توکن استفاده میکنیم کلیم ها در profileService ست می شوند
        // default claims:
        // sub,  email,  AspNet.Identity.SecurityStamp,role,preferred_username,name,email_verified,phone_number,phone_number_verified,idp,amr,auth_time,

        var claims = new List<Claim>
        {
            new("lang", user.Lang),
        };
        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));
        }
        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        await _signInManager.SignInWithClaimsAsync(user, true, claims);

        // چون بهره بردار دارم نمیتونم از روش زیر استفاده کنم به خاطر گلوبال فیلتر
        //  var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);
        return true;


    }

    public async Task<bool> UpdateUser(EditUserCommand input)
    {
        var user = await _userManager.FindByIdAsync(input.Id.ToString());
        if (user == null)
            return false;

        user.FirstName = input.FirstName;
        user.LastName = input.LastName;
        user.FullName = $"{input.FirstName} {input.LastName}";
        user.Email = input.Email;
        user.PhoneNumber = input.PhoneNumber;
        user.Lang = input.Lang;
        user.Address = input.Address;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new CustomValidationException(updateResult.Errors);

        // دریافت رول های فعلی
        var currentRoles = await _userManager.GetRolesAsync(user);

        // حذف رول های قبلی
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            throw new CustomValidationException(removeResult.Errors);

        // اضافه کردن رول های جدید
        var addResult = await _userManager.AddToRolesAsync(user, input.Roles);
        if (!addResult.Succeeded)
            throw new CustomValidationException(addResult.Errors);

        return true;
    }

    public async Task<(Guid id, string roleName)> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        return (role!.Id, role.Name ?? "");
    }

    public async Task<bool> UpdateRole(Guid id, string roleName)
    {
        if (roleName != null)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                role.Name = roleName;
                var result = await _roleManager.UpdateAsync(role);
                return result.Succeeded;
            }
        }
        return false;
    }

    public async Task<bool> UpdateUsersRole(string userName, IList<string> usersRole)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return false;
        }
        var existingRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        result = await _userManager.AddToRolesAsync(user, usersRole);

        return result.Succeeded;
    }



    public async Task<bool> UpdateRolePermissionsAsync(EditRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
            throw new NotFoundException($"Role '{request.RoleName}' not found.");
        // لیست پرمیژن هایی که از قبل به این نقش اختصاص داده شده اند
        var currentPermissions = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.Permission.Name)
            .ToListAsync(cancellationToken);

        var newPermissions = request.GrantedPermissions ?? new List<string>();


        // حذف پرمیژن هایی که در حال حاضر وجود دارند ولی در لیست جدید نیستند
        var permissionsToRemove = currentPermissions.Except(newPermissions, StringComparer.OrdinalIgnoreCase).ToList();
        if (permissionsToRemove.Any())
        {
            var entitiesToRemove = await _dbContext.RolePermissions
                .Include(i => i.Permission)
                .Where(rp => rp.RoleId == role.Id && permissionsToRemove.Contains(rp.Permission.Name))
                .ToListAsync(cancellationToken);

            _dbContext.RolePermissions.RemoveRange(entitiesToRemove);
        }



        // اضافه کردن پرمیژن هایی که جدید هستند
        var permissionsToAdd = newPermissions.Except(currentPermissions, StringComparer.OrdinalIgnoreCase).ToList();
        if (permissionsToAdd.Any())
        {
            // نکته: با اضافه کردن پرمیژن به کد باید حتما update-database زده شود
            var permissionsMustBeAdded = await _dbContext.Permissions
                  .Select(s => new { s.Id, s.Name })
                  .Where(w => permissionsToAdd.Contains(w.Name)).ToListAsync();

            var entitiesToAdd = permissionsMustBeAdded.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            });

            await _dbContext.RolePermissions.AddRangeAsync(entitiesToAdd, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        // _logger.LogInformation("Removed permissions: {Permissions}", string.Join(", ", permissionsToRemove));
        // _logger.LogInformation("Added permissions: {Permissions}", string.Join(", ", permissionsToAdd));
        // TODO:update role cache for users where has this roles
        return true;
    }

    public async Task<bool> updateUserLanguage(string culture, Guid userId, CancellationToken cancellationToken)
    {
        var found = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
        if (found != null)
        {
            found.Lang = culture;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        return false;
    }


    public async Task<(bool EnableTwoFactor, string Secret, int AttemptCount, TwoFactorTypeEnum TwoFactorType)> getUserTwoFactorData(Guid userId, CancellationToken cancellationToken)
    {
        var found = await _dbContext.UserSetting.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        if (found == null)
        {
            return new()
            {
                EnableTwoFactor = false,
                TwoFactorType = default,
                Secret = "",
                AttemptCount = 0,
            };
        }
        var isEnable = found.EnableTwoFactor;
        if (found.IsVerified == false || (isEnable && string.IsNullOrEmpty(found.Secret) && found.TwoFactorType == TwoFactorTypeEnum.AuthenticatorApp))
        {
            isEnable = false;
        }
        return new()
        {
            EnableTwoFactor = isEnable,
            TwoFactorType = found.TwoFactorType,
            Secret = found.Secret,
            AttemptCount = found.AttemptCount
        };
    }

    public async Task<LoginResult> LoginInAsync(LoginCommand request, CancellationToken cancellation)
    {
        Guid? tenantId = null;
        if (string.IsNullOrEmpty(request.Tenant) == false)
        {
            var tenant = await GetTenantByTenancyName(request.Tenant, cancellation);
            if (tenant == null)
            {
                throw new InvalidLoginException("Invalid tenant!");
            }
            tenantId = tenant?.Id;
        }




        var result = await SigninUserAsync(request.UserName, request.Password, tenantId);


        if (!result)
        {
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification(request.UserName, request.Tenant));
            throw new InvalidLoginException();
        }
        UserDTO userData = await GetUserDetailsByUserNameAsync(request.UserName, tenantId);
        // check two factor
        var twoFactor = await getUserTwoFactorData(userData.Id, cancellation);
        if (twoFactor.EnableTwoFactor)
        {
            LoginResult output = new LoginResult()
            {
                User = userData,
                Roles = [],
                EnableTwoFactor = twoFactor.EnableTwoFactor,
                TwoFactorType = twoFactor.TwoFactorType
            };
            return output;
        }
        else
        {
            var permissions = await _userPermissionService.GetUserPermissionsAsync(userData.Id, cancellation);
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(request.UserName, request.Tenant));
            LoginResult output = new LoginResult()
            {
                User = userData,
                Roles = userData.Roles,
                Permissions = permissions
            };
            return output;
        }
    }


    public async Task LogoutAsync(CancellationToken cancellation)
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<LoginResult> DelegateUser(DelegateUserCommand request, CancellationToken cancellation)
    {
        ApplicationUser? delegator =
       await _userManager.FindByIdAsync(_currentUser.UserId!.Value.ToString());

        if (delegator == null)
            throw new UnauthorizedAccessException();


        var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Id == request.UserId &&
                         x.TenantId == request.TenantId &&
                         !x.IsDeleted,
                    cancellation);

        if (user == null)
            throw new NotFoundException("User not found");

        var claims = new List<Claim>
                {
                    new("sub", user.Id.ToString()),
                    new("tenant_id", user.TenantId!.Value.ToString()),
                    new("lang", user.Lang),

                    new("is_delegated", "true"),
                    new("delegator_id", delegator.Id.ToString()),
                    new("delegator_username", delegator.UserName!)
                };
        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellation);

        await _dbContext.UserDelegations.AddAsync(new UserDelegation()
        {
            AdminId = delegator.Id,
            TargetUserId = user.Id,
            IsActive = true,
            Reason = request.Reason ?? "Delegation",
            StartTime = DateTime.UtcNow,
            StartedFromIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
        });
        await _dbContext.SaveChangesAsync(cancellation);

        return new LoginResult
        {
            User = await GetUserDetailsByUserNameAsync(
                user.UserName!,
                user.TenantId),

            Roles =
                (await _userManager.GetRolesAsync(user))
                .ToList(),

            Permissions = permissions
        };
    }

    public async Task ExitDelegation(CancellationToken cancellation)
    {
        var delegatorId = _currentUser.DelegatorId;
        var delegatedUserId = _currentUser.UserId;

        if (delegatorId == null)
            return;

        var admin = await _userManager.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == delegatorId && !x.IsDeleted, cancellation);

        if (admin == null)
            throw new UnauthorizedAccessException();


        var d = await _dbContext.UserDelegations
            .Where(x =>
                    x.AdminId == delegatorId &&
                    x.TargetUserId == delegatedUserId &&
                    x.IsActive == true
                 )
            .OrderByDescending(x => x.StartTime)
            .FirstOrDefaultAsync(cancellation);
        if (d != null)
        {
            d.EndTime = DateTime.UtcNow;
            d.IsActive = false;
            d.EndedFromIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            await _dbContext.SaveChangesAsync(cancellation);
        }
        await _signInManager.SignInAsync(
            admin,
            false);
    }

    public async Task<LoginResult> TwofactorSignInAsync(TwoFactorLoginCommand request, CancellationToken cancellation)
    {
        var tenant = await GetTenantByTenancyName(request.TenancyName, cancellation);
        if (tenant == null)
        {
            throw new InvalidLoginException();
        }
        Guid tenantId = tenant.Id;

        var result = await SigninUserAsync(request.UserName, request.Password, tenantId);

        if (!result)
        {
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification(request.UserName, request.TenancyName));
            throw new InvalidLoginException();
        }
        UserDTO userData = await GetUserDetailsByUserNameAsync(request.UserName, tenantId);
        // verify two factor
        var hostSettings = await _securitySettingService.GetSettingsAsync(cancellation);
        var settings = await _securitySettingService.GetUserSettingsAsync(userData.Id, cancellation);
        var twoFactor = await getUserTwoFactorData(userData.Id, cancellation);

        if (twoFactor.AttemptCount >= hostSettings.LogginAttemptCountLimit)
        {
            throw new LoginAttempCountException();
        }

        var verify = _twoFactorService.VerifyCodeAsync(twoFactor.Secret, request.Code);
        if (verify == true)
        {

            var permissions = await _userPermissionService.GetUserPermissionsAsync(userData.Id, cancellation);

            BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(request.UserName, request.TenancyName));
            LoginResult output = new LoginResult()
            {
                User = userData,
                Roles = userData.Roles,
                Permissions = permissions
            };
            await _twoFactorService.ResetAttemptCount(userData.Id);

            return output;
        }
        else
        {
            await _twoFactorService.SetAttemptCount(userData.Id);
            throw new InvalidTwoFactorCodeException();
        }
    }




    public async Task<LoginResult> ExternalSignInAsync(ExternalLoginCallbackCommand request, CancellationToken cancellation)
    {
        var tenant = await GetTenantByTenancyName(request.tenancyName, cancellation);
        if (tenant == null)
        {
            throw new InvalidLoginException();
        }
        Guid tenantId = tenant.Id;

        //var info = await _signInManager.GetExternalLoginInfoAsync();
        //if (info == null) throw new UnauthorizedAccessException("External login info not found");

        //var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        var backendUrl = _config["BackendUrl"] ?? "";
        if (backendUrl.EndsWith("/") == false)
        {
            backendUrl += "/";
        }

        var provider = await _dbContext.ExternalProviders
            .Where(x => x.Name.ToLower() == request.provider.ToLower() && x.IsActive)
            .Select(s => new
            {
                s.Name,
                s.TokenEndpoint,
                s.ProviderType,
                s.ClientId,
                s.ClientSecret,
            })
            .FirstOrDefaultAsync(cancellation);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("External login provider not found");
        }

        var clientId = provider.ClientId ?? _secretStore.GetSecret(request.provider + ":ClientId");
        var secret = provider.ClientSecret ?? _secretStore.GetSecret(request.provider + ":ClientSecret");
        var redirectUrl = _config["ExternalLogin:RedirectUri"]! + provider.Name;
        if (provider.ProviderType == ExternalProviderTypeEnum.OpenIdConnect)
        {
            redirectUrl = $"{backendUrl}api/externalauth/callback/{provider.Name}";
        }


        var externalLoginResult = await _externalLoginService.ExchangeCodeAsync(
            provider.ProviderType,
            request.code,
            provider.TokenEndpoint,
            clientId,
            secret,
            request.codeVerifier ?? "",
            redirectUrl,
            cancellation);

        ;
        if (externalLoginResult == null)
        {
            throw new UnauthorizedAccessException("External login provider result not found");
        }
        ApplicationUser? user = await FindOrCreateExternalUser(
            externalLoginResult.Email!, externalLoginResult.UserName!, externalLoginResult.Name!, provider.Name, tenantId);
        await _signInManager.SignInAsync(user, false);

        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellation);
        BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(user.UserName!, request.tenancyName));
        LoginResult output = new LoginResult()
        {
            User = new UserDTO
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
                CreationTime = user.CreatedAt,
                Roles = roles.ToList()
            },
            Roles = roles.ToList(),
            Permissions = permissions
        };
        return output;


    }

    public async Task<List<Guid>> GetAllActiveUsersIds(CancellationToken cancellationToken)
    {
        List<Guid> list = await _dbContext.Users
             // .Where(x => x.IsDeleted == false)
             .Select(u => u.Id)
             .ToListAsync();
        return list;
    }

    public async Task<List<AutoCompleteDto<Guid>>> GetAutoCompleteUsers(string filter, CancellationToken cancellationToken)
    {

        var query = _dbContext.Users.AsQueryable();


        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(c =>
               (c.UserName != null && c.UserName.Contains(filter)) ||
                (c.FirstName != null && c.FirstName.Contains(filter)) ||
                c.LastName.Contains(filter)
            );
        }

        var items = await query.OrderBy(x => x.CreatedAt)
            .Skip(0)
            .Take(50)
            .Select(s => new AutoCompleteDto<Guid>
            {
                Id = s.Id,
                Title = s.FirstName + " " + s.LastName + " (" + s.UserName + ")",
            })
            .ToListAsync(cancellationToken);
        return items;
    }

    private async Task<ApplicationUser> FindOrCreateExternalUser(string email, string userName, string name, string providerName, Guid tenantId)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {

            user = new ApplicationUser
            {
                UserName = userName ?? email,
                FullName = name,
                Email = email,
                Lang = AppConsts.DefaultLanguage,
                EmailConfirmed = true
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification("provider: " + providerName, tenantId.ToString()));
                throw new ValidationException(createResult.Errors.Select(e => e.Description).FirstOrDefault());
            }
        }
        return user;
    }

    public async Task<UserDTO?> GetTenantAdmin(Guid tenantId, CancellationToken cancellation)
    {
        var tenantAdminUser = await _dbContext.Users
         .Join(
             _dbContext.UserRoles,
             user => user.Id,
             userRole => userRole.UserId,
             (user, userRole) => new { user, userRole }
         )
         .Join(
             _dbContext.Roles,
             combined => combined.userRole.RoleId,
             role => role.Id,
             (combined, role) => new { combined.user, role }
         )
         .Where(x => x.user.TenantId == tenantId
                  && x.role.Name == AppRoles.TenantAdministrator)
         .Select(x => new UserDTO
         {
             Id = x.user.Id,
             UserName = x.user.UserName ?? "",
             FirstName = x.user.FirstName ?? "",
             LastName = x.user.LastName ?? "",
             FullName = x.user.FullName ?? "",
             Lang = x.user.Lang ?? "",
             Email = x.user.Email ?? "",
             ProfilePicture = x.user.ProfilePicture ?? "",
             Address = x.user.Address ?? "",
             PhoneNumber = x.user.PhoneNumber ?? "",
             Roles = new List<string> { x.role.Name! }
         })
         .FirstOrDefaultAsync(cancellation);

        return tenantAdminUser;

        // with Navigation property
        //return await _dbContext.Users
        //.Where(u => u.TenantId == tenantId)
        //.Where(u => u.UserRoles.Any(ur => ur.Role.Name == Roles.TenantAdministrator))
        //.Select(u => new UserDTO
        //{
        //    Id = u.Id,
        //    UserName = u.UserName ?? "",
        //    FirstName = u.FirstName ?? "",
        //    LastName = u.LastName ?? "",
        //    FullName = u.FullName ?? "",
        //    Lang = u.Lang ?? "",
        //    Email = u.Email ?? "",
        //    ProfilePicture = u.ProfilePicture ?? "",
        //    Address = u.Address ?? "",
        //    PhoneNumber = u.PhoneNumber ?? "",
        //    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        //})
        //.FirstOrDefaultAsync(cancellation);


    }
}
