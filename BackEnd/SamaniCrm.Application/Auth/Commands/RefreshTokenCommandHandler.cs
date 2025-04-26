using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IIdentityService   _identityService;

        public RefreshTokenCommandHandler(ITokenGenerator tokenGenerator, IIdentityService identityService)
        {
            _tokenGenerator = tokenGenerator;
            _identityService = identityService;
        }

        public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.GetUserIdFromRefreshToken(request.RefreshToken);
            if(result.Equals(Guid.Empty))
                throw new ForbiddenAccessException();

            var user= await _identityService.GetUserDetailsAsync(result);
            var accessToken = _tokenGenerator.GenerateAccessToken(user.userId, user.UserName, user.roles);
            var newRefreshToken = await _tokenGenerator.GenerateRefreshToken(user.userId, accessToken);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };

            //var storedToken = await _context.RefreshTokens
            //    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            //if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            //    throw new SecurityTokenException("Invalid refresh token");

            //storedToken.IsUsed = true;
            //_context.RefreshTokens.Update(storedToken);
            //await _context.SaveChangesAsync();

            //var user = await _userManager.FindByIdAsync(storedToken.UserId);

            //return await new GenerateTokenCommandHandler(_userManager, _jwtTokenService, _context)
            //    .Handle(new GenerateTokenCommand(user), cancellationToken);
        }
    }

}
