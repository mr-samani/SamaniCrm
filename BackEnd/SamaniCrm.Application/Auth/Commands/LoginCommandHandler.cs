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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;


        public LoginCommandHandler(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mediator = mediator;
        }


        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new ValidateUserQuery(request.Username, request.Password), cancellationToken);

            if (user == null)
                throw new InvalidLoginException();

            var roles = await _mediator.Send(new GetUserRolesQuery(user), cancellationToken);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            LoginResult output = new LoginResult(
                AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken: "",
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
