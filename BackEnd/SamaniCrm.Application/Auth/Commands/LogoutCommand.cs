using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands;

public class LogoutCommand : IRequest;


public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IIdentityService _identityService;

    public LogoutCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _identityService.LogoutAsync(cancellationToken);
    }
}