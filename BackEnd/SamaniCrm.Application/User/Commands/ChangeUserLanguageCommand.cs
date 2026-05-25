using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.User.Commands
{
    public record ChangeUserLanguageCommand(string culture) : IRequest<bool>;




    public class ChangeUserLanguageCommandHandler : IRequestHandler<ChangeUserLanguageCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IIdentityService _identityService;


        public ChangeUserLanguageCommandHandler(ICurrentUserService currentUser, IApplicationDbContext context, IIdentityService identityService)
        {
            _currentUser = currentUser;
            _context = context;
            this._identityService = identityService;
        }

        public async Task<bool> Handle(ChangeUserLanguageCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
            {
                throw new NotFoundException("User not found");
            }
            var userId = (Guid)_currentUser.UserId!;
            var result = await _identityService.updateUserLanguage(request.culture, userId, cancellationToken);
            if (result == true)
            {
                _currentUser.lang = request.culture;
            }
            return result;

        }
    }

}
