using SamaniCrm.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.User.Commands
{
    public class EditUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Lang { get; set; }
        public string? Address { get; set; }
        public required List<string> Roles { get; set; } = new();
    }

    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public EditUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<bool> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.UpdateUser(request);
            return result;
        }
    }
}
