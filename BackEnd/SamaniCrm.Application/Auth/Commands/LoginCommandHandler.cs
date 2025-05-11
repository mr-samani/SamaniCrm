using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
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
            var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
            {
                BackgroundJob.Enqueue(() => SendLoginFailureNotification(request.UserName));
                throw new InvalidLoginException();
            }
            UserResponseDTO  userData = await _identityService.GetUserDetailsByUserNameAsync(request.UserName);

            var accessToken = _tokenGenerator.GenerateAccessToken(userData.Id,
                userData.UserName,
                userData.Lang,
                userData.Roles);
            var refreshToken = await _tokenGenerator.GenerateRefreshToken(userData.Id, accessToken);

            BackgroundJob.Enqueue(() => SendLoginNotification(request.UserName));
            LoginResult output = new LoginResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userData,
                Roles = userData.Roles
            };
            return output;
        }


        public void SendLoginNotification(string username)
        {
            // کد ارسال نوتیفیکیشن یا لاگ
            Console.WriteLine($"User {username} logged in successfully!");
        }

        public void SendLoginFailureNotification(string username)
        {
            // کد ارسال نوتیفیکیشن یا لاگ
            Console.WriteLine($"User {username} failed to log in.");
        }
    }
}
