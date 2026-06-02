using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands;

public class TwoFactorLoginCommand : IRequest<LoginResult>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Code { get; set; }
    public required string TenancyName { get; set; }
}


public class TwoFactorLoginCommandHandler : IRequestHandler<TwoFactorLoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    public TwoFactorLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }


    public async Task<LoginResult> Handle(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        var output = await _identityService.TwofactorSignInAsync(request, cancellationToken);
        return output;

    }



}
