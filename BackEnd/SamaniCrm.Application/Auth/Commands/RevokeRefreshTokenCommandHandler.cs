using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Auth.Commands
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public RevokeRefreshTokenCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.RevokeRefreshToken(request.Token, cancellationToken);
            return result;
        }
    }

}
