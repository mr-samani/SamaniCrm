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
    private readonly IIdentityService _identityService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly ICaptchaStore _captcha;
    public LoginCommandHandler(
            IIdentityService identityService,
            ICaptchaStore captcha,
            ISecuritySettingService securitySettingService)
    {
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
        var output = await _identityService.SignInAsync(request, cancellationToken);
        return output;


    }



}