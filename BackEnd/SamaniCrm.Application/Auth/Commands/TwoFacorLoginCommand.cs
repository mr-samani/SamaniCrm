using Hangfire;
using MediatR;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Commands;

public class TwoFactorLoginCommand : IRequest<LoginResult>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Code { get; set; }
}


public class TwoFactorLoginCommandHandler : IRequestHandler<TwoFactorLoginCommand, LoginResult>
{
    private readonly IMediator _mediator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly IUserPermissionService _userPermissionService;
    public TwoFactorLoginCommandHandler(
            IMediator mediator,
            ITokenGenerator tokenGenerator,
            IIdentityService identityService,
            ITwoFactorService twoFactorService,
            ISecuritySettingService securitySettingService,
            IUserPermissionService userPermissionService)
    {
        _mediator = mediator;
        _tokenGenerator = tokenGenerator;
        _identityService = identityService;
        _twoFactorService = twoFactorService;
        _securitySettingService = securitySettingService;
        _userPermissionService = userPermissionService;
    }


    public async Task<LoginResult> Handle(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

        if (!result)
        {
            BackgroundJob.Enqueue(() => SendLoginFailureNotification(request.UserName));
            throw new InvalidLoginException();
        }
        UserDTO userData = await _identityService.GetUserDetailsByUserNameAsync(request.UserName);
        // verify two factor
        var hostSettings = await _securitySettingService.GetSettingsAsync(cancellationToken);
        var settings = await _securitySettingService.GetUserSettingsAsync(userData.Id, cancellationToken);
        var twoFactor = await _identityService.getUserTwoFactorData(userData.Id, cancellationToken);

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

            BackgroundJob.Enqueue(() => SendLoginNotification(request.UserName));
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


    public void SendLoginNotification(string username)
    {
        // کد ارسال نوتیفیکیشن یا لاگ
        Console.WriteLine($"User {username} logged in successfully!");
    }

    public void SendLoginFailureNotification(string username)
    {
        // کد ارسال نوتیفیکیشن یا لاگ
        Console.WriteLine($"User {username} failed to log in.");
    }
}
