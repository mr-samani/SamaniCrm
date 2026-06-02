using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands;

public class ExitDelegationCommand : IRequest;

public class ExitDelegationCommandHandler : IRequestHandler<ExitDelegationCommand>
{
    private readonly IIdentityService _identityService;

    public ExitDelegationCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(ExitDelegationCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ExitDelegation(cancellationToken);
    }
}