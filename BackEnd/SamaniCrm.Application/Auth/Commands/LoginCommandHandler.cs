using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Services;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMediator mediator,
            ApplicationDbContext context,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mediator = mediator;
            _context = context;
            _authService = authService;
        }


        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new ValidateUserQuery(request.Username, request.Password), cancellationToken);

            if (user == null)
                throw new InvalidLoginException();

            var roles = await _mediator.Send(new GetUserRolesQuery(user), cancellationToken);

            var expiration = DateTime.UtcNow.AddHours(1);
            var accessToken = await _authService.GenerateAccessToken(user);
            var refreshToken = await _authService.GenerateRefreshToken(user, accessToken);

           
            LoginResult output = new LoginResult(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                AccessTokenExpiration: expiration,
                RefreshTokenExpiration: expiration.AddDays(7), // فرضی
                UserId: user.Id,
                UserName: user.UserName,
                Email: user.Email ?? "",
                FirstName: user.FirstName ?? "",
                LastName: user.LastName ?? "",
                ProfilePicture: user.ProfilePicture ?? "",
                Roles: roles.ToArray()
                );
            return output;
        }
    }
}
