using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Commands;

public record ExternalLoginCallbackCommand(string code,string provider) : IRequest<LoginResult>;

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
