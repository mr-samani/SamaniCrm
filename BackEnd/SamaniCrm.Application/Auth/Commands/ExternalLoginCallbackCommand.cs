using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands;

public record ExternalLoginCallbackCommand(string code,string provider,string tenancyName,string? codeVerifier) : IRequest<LoginResult>;

public class ExternalLoginCallbackHandler : IRequestHandler<ExternalLoginCallbackCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserPermissionService _userPermissionService;

    public ExternalLoginCallbackHandler(IIdentityService identityService,
        ITokenGenerator tokenGenerator,
        IUserPermissionService userPermissionService)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _userPermissionService = userPermissionService;
    }



    public async Task<LoginResult> Handle(ExternalLoginCallbackCommand cmd, CancellationToken cancellationToken)
    {
        var output = await _identityService.ExternalSignInAsync(cmd, cancellationToken);
        return output;
    }
}
