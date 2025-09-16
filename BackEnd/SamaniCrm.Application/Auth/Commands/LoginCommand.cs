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

public class LoginCommand : IRequest<LoginResult>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public InputCaptchaDTO? captcha { get; set; }
}


public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IMediator _mediator;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly ICaptchaStore _captcha;
    private readonly ISecuritySettingService _securitySettingService;
    public LoginCommandHandler(
            IMediator mediator,
            ITokenGenerator tokenGenerator,
            IIdentityService identityService,
            ICaptchaStore captcha,
            ISecuritySettingService securitySettingService)
    {
        _mediator = mediator;
        _tokenGenerator = tokenGenerator;
        _identityService = identityService;
        _captcha = captcha;
        _securitySettingService = securitySettingService;
    }


    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var securitySettings = await _securitySettingService.GetSettingsAsync(cancellationToken);
        if (securitySettings.RequireCaptchaOnLogin)
        {
            if (request.captcha == null || string.IsNullOrEmpty(request.captcha.CaptchaKey) || string.IsNullOrEmpty(request.captcha.CaptchaText))
            {
                throw new InvalidCaptchaException();
            }
            var captchaValid = _captcha.ValidateCaptcha(request.captcha.CaptchaKey, request.captcha.CaptchaText);
            if (captchaValid == false)
            {
                throw new InvalidCaptchaException();
            }
        }
        var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

        if (!result)
        {
            BackgroundJob.Enqueue(() => SendLoginFailureNotification(request.UserName));
            throw new InvalidLoginException();
        }
        UserDTO userData = await _identityService.GetUserDetailsByUserNameAsync(request.UserName);
        // check two factor
        var twoFactor = await _identityService.getUserTwoFactorData(userData.Id, cancellationToken);
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

            BackgroundJob.Enqueue(() => SendLoginNotification(request.UserName));
            LoginResult output = new LoginResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userData,
                Roles = userData.Roles
            };
            return output;
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