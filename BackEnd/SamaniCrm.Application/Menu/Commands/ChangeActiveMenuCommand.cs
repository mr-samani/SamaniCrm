using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Menu.Commands
{

    public record ChangeActiveMenuCommand(Guid Id, bool IsActive) : IRequest<bool>;

    public class ChangeActiveMenuCommandHandler : IRequestHandler<ChangeActiveMenuCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public ChangeActiveMenuCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(ChangeActiveMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _dbContext.Menus.FindAsync(request.Id);
            if (menu == null)
                throw new NotFoundException("Menu not found.");

            menu.IsActive = request.IsActive;
            menu.LastModifiedTime = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }


}
