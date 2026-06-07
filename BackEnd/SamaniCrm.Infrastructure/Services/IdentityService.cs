using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.ExternalLogin;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.TenantManager;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using UnauthorizedAccessException = SamaniCrm.Application.Common.Exceptions.UnauthorizedAccessException;


namespace SamaniCrm.Infrastructure.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly TenantDbContext _dbContext;
    private readonly MasterDbContext _masterDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserPermissionService _userPermissionService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IExternalLoginService _externalLoginService;
    private readonly ISecretStore _secretStore;
    private readonly IConfiguration _config;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IHostIdentityService _hostIdentityService;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        TenantDbContext dbContext,
        MasterDbContext masterDbContext,
        IHttpContextAccessor httpContextAccessor,
        IUserPermissionService userPermissionService,
        ISecuritySettingService securitySettingService,
        ITwoFactorService twoFactorService,
        IExternalLoginService externalLoginService,
        ISecretStore secretStore,
        IConfiguration config,
        ICurrentUserService currentUser,
        IHostIdentityService hostIdentityService,
        ITenantDbContextFactory tenantDbContextFactory,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentTenant currentTenant)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _masterDbContext = masterDbContext;
        _httpContextAccessor = httpContextAccessor;
        _userPermissionService = userPermissionService;
        _securitySettingService = securitySettingService;
        _twoFactorService = twoFactorService;
        _externalLoginService = externalLoginService;
        _secretStore = secretStore;
        _config = config;
        _currentUser = currentUser;
        _hostIdentityService = hostIdentityService;
        _tenantDbContextFactory = tenantDbContextFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _currentTenant = currentTenant;
    }



    public async Task<List<(Guid id, string roleName)>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();

        return roles.Select(role => (role.Id, role.Name!)).ToList();
    }

    public async Task<List<string>> GetUserRolesByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken)
    {
        var user = await FindUserByUserNameAsync(userName, tenantId, cancellationToken);
        if (user is null)
            return [];

        return (await _userManager.GetRolesAsync(user)).ToList();
    }

    public async Task<UserDTO> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return MapUser(user, roles.ToList());
    }

    public async Task<UserDTO> GetUserDetailsByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken)
    {
        var user = await FindUserByUserNameAsync(userName, tenantId, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return MapUser(user, roles.ToList());
    }

    public async Task<bool> IsUniqueUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _userManager.Users
            .IgnoreQueryFilters()
            .AllAsync(x => x.UserName != userName, cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found");

        return await _userManager.IsInRoleAsync(user, role);
    }




    public async Task<PaginatedResult<UserDTO>> GetAllUsersAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        //var rolesQuery = from ur in _dbContext.UserRoles
        //                 join r in _roleManager.Roles on ur.RoleId equals r.Id
        //                 select new { ur.UserId, RoleName = r.Name };

        var rolesQuery = await _dbContext.Database
                 .SqlQueryRaw<UserRoleDto>(@"
                    SELECT 
                        ur.UserId,
                        r.Name AS RoleName
                    FROM AspNetUserRoles ur
                    INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
                ")
                .ToListAsync();

        IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();
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
                TenantId = u.TenantId,
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

    public async Task<(bool isSucceed, Guid userId)> CreateUserAsync(CreateUserCommand input, CancellationToken cancellationToken)
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
    public async Task<bool> UpdateUserAsync(EditUserCommand input, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == input.Id && !x.IsDeleted, cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found");

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

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            throw new CustomValidationException(removeResult.Errors);

        var addResult = await _userManager.AddToRolesAsync(user, input.Roles);
        if (!addResult.Succeeded)
            throw new CustomValidationException(addResult.Errors);

        return true;
    }
    public async Task<bool> UpdateUserRoles(string userName, IList<string> usersRole, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return false;
        }
        var existingRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        result = await _userManager.AddToRolesAsync(user, usersRole);

        return result.Succeeded;
    }

    public async Task<bool> updateUserLanguage(string culture, Guid userId, CancellationToken cancellationToken)
    {
        var found = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
        if (found != null)
        {
            found.Lang = culture;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        return false;
    }



    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found");

        if (user.UserName is "system" or "admin")
            throw new BadRequestException("You can not delete system or admin user");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<(Guid id, string roleName)> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
            throw new NotFoundException("Role not found");

        return (role.Id, role.Name ?? string.Empty);
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

    public async Task<bool> AssigRolesToUser(string userName, IList<string> roles)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.AddToRolesAsync(user, roles);
        return result.Succeeded;
    }


    public async Task<bool> UpdateRoleAsync(Guid id, string roleName, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
            return false;

        role.Name = roleName;
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
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


    public async Task<bool> UpdateRolePermissionsAsync(EditRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.Roles.Where(x => x.Name == request.RoleName).FirstOrDefaultAsync();
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


    public async Task<LoginResult> LoginAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _hostIdentityService.GetTenantByTenancyName(request.TenancyName, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant not found");


        using (_currentTenant.Change(
            tenant.Id,
            tenant.TenancyName,
            tenant.TenantName,
            tenant.ConnectionString))
        {

            var user = await FindUserByUserNameAsync(request.UserName, tenant?.Id, cancellationToken);
            if (user is null)
                throw new InvalidLoginException();

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
                throw new InvalidLoginException();

            var roles = await _userManager.GetRolesAsync(user);
            var dto = MapUser(user, roles.ToList());

            var twoFactor = await GetTwoFactorDataAsync(user.Id, cancellationToken);
            if (twoFactor.EnableTwoFactor)
            {
                return new LoginResult
                {
                    User = dto,
                    Roles = [],
                    EnableTwoFactor = true,
                    TwoFactorType = twoFactor.TwoFactorType
                };
            }

            var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

            await SignInWithClaimsAsync(user, roles.ToList(), cancellationToken);

            return new LoginResult
            {
                User = dto,
                Roles = roles.ToList(),
                Permissions = permissions
            };
        }
    }

    public async Task<LoginResult> TwoFactorLoginAsync(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _hostIdentityService.GetTenantByTenancyName(request.TenancyName, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant not found");


        using (_currentTenant.Change(
            tenant.Id,
            tenant.TenancyName,
            tenant.TenantName,
            tenant.ConnectionString))
        {

            var user = await FindUserByUserNameAsync(request.UserName, tenant.Id, cancellationToken);
            if (user is null)
                throw new InvalidLoginException();

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
                throw new InvalidLoginException();

            var hostSettings = await _securitySettingService.GetSettingsAsync(cancellationToken);
            var twoFactor = await GetTwoFactorDataAsync(user.Id, cancellationToken);

            if (twoFactor.AttemptCount >= hostSettings?.LogginAttemptCountLimit)
                throw new LoginAttempCountException();

            if (!_twoFactorService.VerifyCodeAsync(twoFactor.Secret, request.Code))
            {
                await _twoFactorService.SetAttemptCount(user.Id);
                throw new InvalidTwoFactorCodeException();
            }

            await _twoFactorService.ResetAttemptCount(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

            await SignInWithClaimsAsync(user, roles.ToList(), cancellationToken);

            return new LoginResult
            {
                User = MapUser(user, roles.ToList()),
                Roles = roles.ToList(),
                Permissions = permissions
            };
        }
    }


    public async Task<LoginResult> ExternalLoginAsync(ExternalLoginCallbackCommand request, CancellationToken cancellationToken)
    {
        var tenant = await ResolveTenantByTenancyNameAsync(request.tenancyName, cancellationToken);
        if (tenant is null)
            throw new InvalidLoginException();

        var backendUrl = _config["BackendUrl"] ?? "";
        if (backendUrl.EndsWith("/") == false)
        {
            backendUrl += "/";
        }

        var provider = await _masterDbContext.ExternalProviders
            .Where(x => x.Name.ToLower() == request.provider.ToLower() && x.IsActive)
            .Select(s => new
            {
                s.Name,
                s.TokenEndpoint,
                s.ProviderType,
                s.ClientId,
                s.ClientSecret,
            })
            .FirstOrDefaultAsync(cancellationToken);
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
            cancellationToken);

        ;
        if (externalLoginResult == null)
        {
            throw new UnauthorizedAccessException("External login provider result not found");
        }
        ApplicationUser? user = await FindOrCreateExternalUser(
            externalLoginResult.Email!,
            externalLoginResult.UserName!,
            externalLoginResult.Name!,
            provider.Name,
            tenant?.Id,
            cancellationToken);
        await _signInManager.SignInAsync(user, false);

        LoginResult output = new LoginResult()
        {
        };
        return output;


    }



    public async Task<LoginResult> DelegateUserAsync(DelegateUserCommand request, CancellationToken cancellationToken)
    {
        var delegator = await FindUserByIdAsync(_currentUser.UserId!.Value, cancellationToken);
        if (delegator is null)
            throw new UnauthorizedAccessException();

        var tenant = await _hostIdentityService.GetTenantById(request.TenantId, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant not found");


        using (_currentTenant.Change(
            tenant.Id,
            tenant.TenancyName,
            tenant.TenantName,
            tenant.ConnectionString))
        {

            var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.UserId && x.TenantId == request.TenantId && !x.IsDeleted, cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

            var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("tenant_id", user.TenantId!.Value.ToString()),
            new("lang", user.Lang ?? string.Empty),

            new("is_delegated", "true"),
            new("delegator_id", delegator.Id.ToString()),
            new("delegator_username", delegator.UserName!)
        };

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            await _dbContext.UserDelegations.AddAsync(new UserDelegation
            {
                AdminId = delegator.Id,
                TargetUserId = user.Id,
                IsActive = true,
                Reason = request.Reason ?? "Delegation",
                StartTime = DateTime.UtcNow,
                StartedFromIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new LoginResult
            {
                User = MapUser(user, roles.ToList()),
                Roles = roles.ToList(),
                Permissions = permissions
            };
        }
    }

    public async Task ExitDelegationAsync(CancellationToken cancellationToken)
    {
        var delegatorId = _currentUser.DelegatorId;
        var delegatedUserId = _currentUser.UserId;

        if (delegatorId is null || delegatedUserId is null)
            return;

        var admin = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == delegatorId && !x.IsDeleted, cancellationToken);

        if (admin is null)
            throw new UnauthorizedAccessException();

        var activeDelegation = await _dbContext.UserDelegations
            .Where(x => x.AdminId == delegatorId &&
                        x.TargetUserId == delegatedUserId &&
                        x.IsActive)
            .OrderByDescending(x => x.StartTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeDelegation is not null)
        {
            activeDelegation.EndTime = DateTime.UtcNow;
            activeDelegation.IsActive = false;
            activeDelegation.EndedFromIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await _signInManager.SignInAsync(admin, false);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<PaginatedResult<TenantUserDTO>> GetTenantUsersAsync(GetTenantUsersQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _hostIdentityService.GetTenantById(request.TenantId, cancellationToken);
        if (tenant is null)
            throw new NotFoundException("Tenant not found");


        using (_currentTenant.Change(
            tenant.Id,
            tenant.TenancyName,
            tenant.TenantName,
            tenant.ConnectionString))
        {
            IQueryable<ApplicationUser> query = _userManager.Users
                      .IgnoreQueryFilters()
                      .Where(x => !x.IsDeleted && x.TenantId == request.TenantId);

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(x =>
                    x.UserName!.Contains(request.Filter) ||
                    x.FirstName!.Contains(request.Filter) ||
                    x.LastName!.Contains(request.Filter) ||
                    x.Email!.Contains(request.Filter) ||
                    x.PhoneNumber!.Contains(request.Filter));
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var sortString = $"{request.SortBy} {request.SortDirection}";
                query = query.OrderBy(sortString);
            }
            else
            {
                query = query.OrderBy(x => x.CreatedAt);
            }

            var total = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .Select(u => new TenantUserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    FullName = u.FullName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Roles = u.Roles.Select(r => r.Name!).ToList()
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
        ;

    }

    public async Task<List<AutoCompleteDto<Guid>>> GetAutoCompleteUsersAsync(string? filter, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(c =>
                (c.UserName != null && c.UserName.Contains(filter)) ||
                (c.FirstName != null && c.FirstName.Contains(filter)) ||
                (c.LastName != null && c.LastName.Contains(filter)));
        }

        return await query
            .OrderBy(x => x.CreatedAt)
            .Take(50)
            .Select(s => new AutoCompleteDto<Guid>
            {
                Id = s.Id,
                Title = $"{s.FirstName} {s.LastName} ({s.UserName})"
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetAllActiveUsersIdsAsync(CancellationToken cancellationToken)
    {
        return await _userManager.Users
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<SimpleTenantData?> ResolveTenantAsync(string? tenancyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenancyName))
            return null;

        return await _hostIdentityService.GetTenantByTenancyName(tenancyName, cancellationToken);
    }

    private async Task<SimpleTenantData?> ResolveTenantByTenancyNameAsync(string tenancyName, CancellationToken cancellationToken)
        => await _hostIdentityService.GetTenantByTenancyName(tenancyName, cancellationToken);

    private async Task SignInWithClaimsAsync(ApplicationUser user, List<string> roles, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new("lang", user.Lang ?? string.Empty)
        };

        if (user.TenantId.HasValue)
            claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));

        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

        await _signInManager.SignInWithClaimsAsync(user, true, claims);
    }

    private async Task<(bool EnableTwoFactor, string Secret, int AttemptCount, TwoFactorTypeEnum TwoFactorType)> GetTwoFactorDataAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var found = await _dbContext.UserSetting
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (found is null)
        {
            return (false, string.Empty, 0, default);
        }

        var enabled = found.EnableTwoFactor;
        if (found.IsVerified == false ||
            (enabled && string.IsNullOrEmpty(found.Secret) && found.TwoFactorType == TwoFactorTypeEnum.AuthenticatorApp))
        {
            enabled = false;
        }

        return (enabled, found.Secret, found.AttemptCount, found.TwoFactorType);
    }

    private async Task<ApplicationUser?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, cancellationToken);
    }

    private async Task<ApplicationUser?> FindUserByUserNameAsync(string userName, Guid? tenantId, CancellationToken cancellationToken)
    {
        if (tenantId.HasValue)
        {
            var tenant = await _hostIdentityService.GetTenantById(tenantId.Value, cancellationToken);
            if (tenant is null)
                return null;
            using (_currentTenant.Change(
                      tenant.Id,
                      tenant.TenancyName,
                      tenant.TenantName,
                      tenant.ConnectionString))
            {
                return await _userManager.Users
                   .IgnoreQueryFilters()
                   .FirstOrDefaultAsync(x => x.UserName == userName && x.TenantId == tenantId && !x.IsDeleted, cancellationToken);
            }
        }
        return await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserName == userName && x.TenantId == null && !x.IsDeleted, cancellationToken);
    }


    private async Task<ApplicationUser> FindOrCreateExternalUser(string email, string userName, string name, string providerName, Guid? tenantId, CancellationToken cancellationToken)
    {
        var user = await FindUserByUserNameAsync(userName, tenantId, cancellationToken);
        if (user == null)
        {

            user = new ApplicationUser
            {
                UserName = userName ?? email,
                FullName = name,
                Email = email,
                Lang = AppConsts.DefaultLanguage,
                EmailConfirmed = true,
                TenantId = tenantId
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new ValidationException(createResult.Errors.Select(e => e.Description).FirstOrDefault());
            }
        }
        return user;
    }


    private static UserDTO MapUser(ApplicationUser user, List<string> roles)
    {
        return new UserDTO
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Lang = user.Lang ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfilePicture = user.ProfilePicture ?? string.Empty,
            Address = user.Address ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CreationTime = user.CreatedAt,
            Roles = roles
        };
    }
}
