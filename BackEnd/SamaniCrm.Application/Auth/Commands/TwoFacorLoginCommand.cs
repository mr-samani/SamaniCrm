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

public class TwoFactorLoginCommand : IRequest<LoginResult>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Code { get; set; }
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
