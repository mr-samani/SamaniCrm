using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IMediator _mediator;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(
                IMediator mediator,
                ITokenGenerator tokenGenerator,
                IIdentityService identityService)
        {
            _mediator = mediator;
            _tokenGenerator = tokenGenerator;
            _identityService = identityService;
        }


        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        { 
          var result= await _identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
                throw new InvalidLoginException();
            var userData= await _identityService.GetUserDetailsByUserNameAsync(request.UserName);

            var accessToken = _tokenGenerator.GenerateAccessToken(userData.userId,userData.UserName,userData.roles);
            var refreshToken = await _tokenGenerator.GenerateRefreshToken(userData.userId,accessToken);


            LoginResult output = new LoginResult(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                UserId: userData.userId,
                UserName: userData.UserName,
                Email: userData.email ?? "",
                FullName:userData.fullName ?? "",
                ProfilePicture: userData.profilePicture ?? "",
                Roles: userData.roles.ToArray()
                );
            return output;
        }
    }
}
