using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Commands;

public record ExternalLoginCallbackCommand() : IRequest<AuthResultDto>;

public class ExternalLoginCallbackHandler : IRequestHandler<ExternalLoginCallbackCommand, AuthResultDto>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwt;

    public ExternalLoginCallbackHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwt)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<AuthResultDto> Handle(ExternalLoginCallbackCommand cmd, CancellationToken ct)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) throw new UnauthorizedAccessException("External login info not found");

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        ApplicationUser user;

        if (result.Succeeded)
        {
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email ?? Guid.NewGuid().ToString(), Email = email };
                var create = await _userManager.CreateAsync(user);
                if (!create.Succeeded) throw new ValidationException(create.Errors.Select(e => e.Description));
            }
            await _userManager.AddLoginAsync(user, info);
        }

        var token = _jwt.Generate(user);
        return new AuthResultDto(user.Id, token);
    }
}
public record AuthResultDto(Guid UserId, string AccessToken);
