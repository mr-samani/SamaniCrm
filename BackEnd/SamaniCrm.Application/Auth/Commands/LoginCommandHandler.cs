using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<IUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IDbContext _context;
        private readonly IAuthService _authService;

        public LoginCommandHandler(UserManager<IUser> userManager,
            IConfiguration configuration,
            IMediator mediator,
            IDbContext context,
            IAuthService authService)
        {
            _userManager = userManager;
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
            var accessToken = _authService.GenerateAccessToken(user);
            var refreshToken = await _authService.GenerateRefreshToken(user, accessToken,cancellationToken);

           
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
