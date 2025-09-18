using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Commands;
public record ExternalLoginCommand(string Provider, string ReturnUrl) : IRequest<string>;

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, string>
 {

    public async Task<string> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        var redirectUrl = $"/api/account/external-login-callback?returnUrl={Uri.EscapeDataString(request.ReturnUrl)}";
        // var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);

        // تولید URL برای Redirect به Provider
        return "";// properties.RedirectUri!;
    }
}

