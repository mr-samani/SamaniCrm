using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Auth.Commands;

public class DelegateUserCommand : IRequest<LoginResult>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string? Reason { get; set; }
}


public class DelegateUserCommandHandler : IRequestHandler<DelegateUserCommand, LoginResult>
{

    private readonly IIdentityService _identityService;

    public DelegateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LoginResult> Handle(DelegateUserCommand request, CancellationToken cancellationToken)
    {
        var result= await _identityService.DelegateUser(request, cancellationToken);
        return result;
    }
}
