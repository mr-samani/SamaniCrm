using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Identity.Interfaces;

namespace SamaniCrm.Application.Auth.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
    {
        private readonly IDbContext _context;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenCommandHandler(IDbContext context, IAuthService authService, IUserRepository userRepository)
        {
            _context = context;
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(q => q.RefreshTokenValue == request.RefreshToken);

            // Refresh token no existe, expiró o fue revocado manualmente
            // (Pensando que el usuario puede dar click en "Cerrar Sesión en todos lados" o similar)
            if (refreshToken is null ||
                refreshToken.Active == false ||
                refreshToken.Expiration <= DateTime.UtcNow)
            {
                throw new ForbiddenAccessException();
            }

            // Se está intentando usar un Refresh Token que ya fue usado anteriormente,
            // puede significar que este refresh token fue robado.
            if (refreshToken.Used)
            {
                // _logger.LogWarning("El refresh token del {UserId} ya fue usado. RT={RefreshToken}", refreshToken.UserId, refreshToken.RefreshTokenValue);

                var refreshTokens = await _context.RefreshTokens
                    .Where(q => q.Active && q.Used == false && q.UserId == refreshToken.UserId)
                    .ToListAsync();

                foreach (var rt in refreshTokens)
                {
                    rt.Used = true;
                    rt.Active = false;
                }

                await _context.SaveChangesAsync(cancellationToken);

                throw new ForbiddenAccessException();
            }

            // TODO: Podríamos validar que el Access Token sí corresponde al mismo usuario

            refreshToken.Used = true;

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);

            if (user is null)
            {
                throw new ForbiddenAccessException();
            }

            var accessToken = _authService.GenerateAccessToken(user);
            var newRefreshToken = await _authService.GenerateRefreshToken(user, accessToken,cancellationToken);

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
