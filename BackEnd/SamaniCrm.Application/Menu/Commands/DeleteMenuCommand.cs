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
    public record DeleteMenuCommand(Guid Id) : IRequest<bool>;
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteMenuCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _dbContext.Menus.FindAsync(request.Id);
            if (menu == null)
                throw new NotFoundException("Menu not found.");

            menu.IsDeleted = true;
            menu.DeletedTime = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }

}
