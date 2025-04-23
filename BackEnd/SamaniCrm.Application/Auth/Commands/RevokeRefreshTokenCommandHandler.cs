using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
    {
        private readonly IDbContext _context;

        public RevokeRefreshTokenCommandHandler(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>  rt.AccessToken == request.Token, cancellationToken);

            if (token == null || token.Active == false)
                return false;

            token.Active = false;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
