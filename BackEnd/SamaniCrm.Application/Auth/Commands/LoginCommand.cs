using MediatR;
using SamaniCrm.Application.Auth.Events;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands;

public class LoginCommand : IRequest<LoginResult>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public InputCaptchaDTO? captcha { get; set; }

    public string? Tenant { get; set; } = null;

    public bool RememberMe { get; set; }

}


public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly ISecuritySettingService _securitySettingService;
    private readonly ICaptchaStore _captcha;
    private readonly IMediator _mediator;
    public LoginCommandHandler(
            IIdentityService identityService,
            ICaptchaStore captcha,
            ISecuritySettingService securitySettingService,
            IMediator mediator)
    {
        _identityService = identityService;
        _captcha = captcha;
        _securitySettingService = securitySettingService;
        _mediator = mediator;
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
        var output = await _identityService.LoginInAsync(request, cancellationToken);
        await _mediator.Publish(
            new UserLogedInEvent(output.User.Id, output.User.UserName, output.User.TenantId,
            $"User: {output.User.FullName} loged in successfully."),
            cancellationToken);
        return output;


    }



}