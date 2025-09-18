using Azure.Core;
using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Models;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Notifications;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading;
using UnauthorizedAccessException = SamaniCrm.Application.Common.Exceptions.UnauthorizedAccessException;


namespace SamaniCrm.Infrastructure.Services;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserPermissionService _userPermissionService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly HttpClient _httpClient;
    private readonly ISecretStore _secretStore;
    private readonly IConfiguration _config;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext applicationDbContext,
        IHttpContextAccessor httpContextAccessor,
        ITokenGenerator tokenGenerator,
        IUserPermissionService userPermissionService,
        ISecuritySettingService securitySettingService,
        ITwoFactorService twoFactorService,
        HttpClient httpClient,
        ISecretStore secretStore,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _applicationDbContext = applicationDbContext;
        _httpContextAccessor = httpContextAccessor;
        _tokenGenerator = tokenGenerator;
        _userPermissionService = userPermissionService;
        _securitySettingService = securitySettingService;
        _twoFactorService = twoFactorService;
        _httpClient = httpClient;
        _secretStore = secretStore;
        _config = config;
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
        var rolesQuery = from ur in _applicationDbContext.UserRoles
                         join r in _applicationDbContext.Roles on ur.RoleId equals r.Id
                         select new { ur.UserId, RoleName = r.Name };

        IQueryable<ApplicationUser> query = _applicationDbContext.Users.AsQueryable();
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
                CreationTime = u.CreationTime.ToUniversalTime(),
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


    public async Task<List<(Guid id, string roleName)>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();

        return roles.Select(role => (role.Id, role.Name)).ToList();
    }

    public async Task<UserDTO> GetUserDetailsAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
            CreationTime = user.CreationTime.ToUniversalTime(),
            Roles = roles.ToList(),
        });
    }

    public async Task<UserDTO> GetUserDetailsByUserNameAsync(string userName)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
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
            CreationTime = user.CreationTime.ToUniversalTime(),
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

    private async Task<bool> SigninUserAsync(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);
        return result.Succeeded;


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

    public async Task<bool> UpdateRolePermissionsAsync(EditRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
            throw new NotFoundException($"Role '{request.RoleName}' not found.");
        // لیست پرمیژن هایی که از قبل به این نقش اختصاص داده شده اند
        var currentPermissions = await _applicationDbContext.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.Permission.Name)
            .ToListAsync(cancellationToken);

        var newPermissions = request.GrantedPermissions ?? new List<string>();


        // حذف پرمیژن هایی که در حال حاضر وجود دارند ولی در لیست جدید نیستند
        var permissionsToRemove = currentPermissions.Except(newPermissions, StringComparer.OrdinalIgnoreCase).ToList();
        if (permissionsToRemove.Any())
        {
            var entitiesToRemove = await _applicationDbContext.RolePermissions
                .Include(i => i.Permission)
                .Where(rp => rp.RoleId == role.Id && permissionsToRemove.Contains(rp.Permission.Name))
                .ToListAsync(cancellationToken);

            _applicationDbContext.RolePermissions.RemoveRange(entitiesToRemove);
        }



        // اضافه کردن پرمیژن هایی که جدید هستند
        var permissionsToAdd = newPermissions.Except(currentPermissions, StringComparer.OrdinalIgnoreCase).ToList();
        if (permissionsToAdd.Any())
        {
            // نکته: با اضافه کردن پرمیژن به کد باید حتما update-database زده شود
            var permissionsMustBeAdded = await _applicationDbContext.Permissions
                  .Select(s => new { s.Id, s.Name })
                  .Where(w => permissionsToAdd.Contains(w.Name)).ToListAsync();

            var entitiesToAdd = permissionsMustBeAdded.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            });

            await _applicationDbContext.RolePermissions.AddRangeAsync(entitiesToAdd, cancellationToken);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        // _logger.LogInformation("Removed permissions: {Permissions}", string.Join(", ", permissionsToRemove));
        // _logger.LogInformation("Added permissions: {Permissions}", string.Join(", ", permissionsToAdd));
        // TODO:update role cache for users where has this roles
        return true;
    }

    public async Task<bool> updateUserLanguage(string culture, Guid userId, CancellationToken cancellationToken)
    {
        var found = await _applicationDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
        if (found != null)
        {
            found.Lang = culture;
            var result = await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        return false;
    }


    public async Task<(bool EnableTwoFactor, string Secret, int AttemptCount, TwoFactorTypeEnum TwoFactorType)> getUserTwoFactorData(Guid userId, CancellationToken cancellationToken)
    {
        var found = await _applicationDbContext.UserSetting.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
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

    public async Task<LoginResult> SignInAsync(LoginCommand request, CancellationToken cancellationToken)
    {


        var result = await SigninUserAsync(request.UserName, request.Password);

        if (!result)
        {
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification(request.UserName));
            throw new InvalidLoginException();
        }
        UserDTO userData = await GetUserDetailsByUserNameAsync(request.UserName);
        // check two factor
        var twoFactor = await getUserTwoFactorData(userData.Id, cancellationToken);
        if (twoFactor.EnableTwoFactor)
        {
            LoginResult output = new LoginResult()
            {
                AccessToken = "",
                RefreshToken = "",
                User = userData,
                Roles = [],
                EnableTwoFactor = twoFactor.EnableTwoFactor,
                TwoFactorType = twoFactor.TwoFactorType
            };
            return output;
        }
        else
        {
            var accessToken = _tokenGenerator.GenerateAccessToken(userData.Id,
                               userData.UserName,
                               userData.Lang,
                               userData.Roles);
            var refreshToken = await _tokenGenerator.GenerateRefreshToken(userData.Id, accessToken);
            var permissions = await _userPermissionService.GetUserPermissionsAsync(userData.Id, cancellationToken);
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(request.UserName));
            LoginResult output = new LoginResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userData,
                Roles = userData.Roles,
                Permissions = permissions
            };
            return output;
        }
    }

    public async Task<LoginResult> TwofactorSignInAsync(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        var result = await SigninUserAsync(request.UserName, request.Password);

        if (!result)
        {
            BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification(request.UserName));
            throw new InvalidLoginException();
        }
        UserDTO userData = await GetUserDetailsByUserNameAsync(request.UserName);
        // verify two factor
        var hostSettings = await _securitySettingService.GetSettingsAsync(cancellationToken);
        var settings = await _securitySettingService.GetUserSettingsAsync(userData.Id, cancellationToken);
        var twoFactor = await getUserTwoFactorData(userData.Id, cancellationToken);

        if (twoFactor.AttemptCount >= hostSettings.LogginAttemptCountLimit)
        {
            throw new LoginAttempCountException();
        }

        var verify = _twoFactorService.VerifyCodeAsync(twoFactor.Secret, request.Code);
        if (verify == true)
        {
            var accessToken = _tokenGenerator.GenerateAccessToken(userData.Id,
                               userData.UserName,
                               userData.Lang,
                               userData.Roles);
            var refreshToken = await _tokenGenerator.GenerateRefreshToken(userData.Id, accessToken);
            var permissions = await _userPermissionService.GetUserPermissionsAsync(userData.Id, cancellationToken);

            BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(request.UserName));
            LoginResult output = new LoginResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
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




    public async Task<LoginResult> ExternalSignInAsync(ExternalLoginCallbackCommand request, CancellationToken cancellationToken)
    {
        //var info = await _signInManager.GetExternalLoginInfoAsync();
        //if (info == null) throw new UnauthorizedAccessException("External login info not found");

        //var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        var provider = await _applicationDbContext.ExternalProviders
            .Where(x => x.Name == request.provider)
            .Select(s => new
            {
                s.Name,
                s.TokenEndpoint,
                s.ProviderType,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (provider == null)
        {
            throw new UnauthorizedAccessException("External login provider not found");
        }

        var clientId = _secretStore.GetSecret(request.provider + ":ClientId");
        var secret = _secretStore.GetSecret(request.provider + ":ClientSecret");
        var redirectUrl = _config["ExternalLogin:RedirectUri"]! + provider.Name;
        var response = await _httpClient.PostAsync(provider.TokenEndpoint,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id",clientId },
                    { "client_secret", secret },
                    { "code", request.code },
                    { "redirect_uri",  redirectUrl},
                    { "grant_type", "authorization_code" }
                }), cancellationToken);
        var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);


        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ExternalLoginException(tokenResult.error_description);
        }




        string access_token = tokenResult!.AccessToken;
        string idToken = tokenResult.IdToken;

        // 3. استخراج ایمیل کاربر از id_token (JWT)
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(idToken);
        var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                    ?? jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;

        if (email == null)
            throw new ValidationException("Email not found in external login");


        ApplicationUser? user = await _userManager.FindByEmailAsync(email ?? "");
        if (user == null)
        {

            user = new ApplicationUser
            {
                UserName = email,
                FullName = name,
                Email = email,
                Lang = AppConsts.DefaultLanguage,
                EmailConfirmed = true
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                BackgroundJob.Enqueue(() => LoginNotification.SendLoginFailureNotification("provider: " + request.provider));
                throw new ValidationException(createResult.Errors.Select(e => e.Description).FirstOrDefault());
            }
        }
        await _signInManager.SignInAsync(user, false);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenGenerator.GenerateAccessToken(user.Id,
                         user.UserName!,
                         user.Lang,
                         roles);
        var refreshToken = await _tokenGenerator.GenerateRefreshToken(user.Id, accessToken);
        var permissions = await _userPermissionService.GetUserPermissionsAsync(user.Id, cancellationToken);
        BackgroundJob.Enqueue(() => LoginNotification.SendLoginNotification(user.UserName!));
        LoginResult output = new LoginResult()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
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
                CreationTime = user.CreationTime.ToUniversalTime(),
                Roles = roles.ToList()
            },
            Roles = roles.ToList(),
            Permissions = permissions
        };
        return output;


    }




    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = default!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        public string? Error { get; set; }
        public string? error_description { get; set; }
    }


}
