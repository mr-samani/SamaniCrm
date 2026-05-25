using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.MenuCommands
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

            var now= DateTime.UtcNow;
            menu.IsDeleted = true;

            var translations = await _dbContext.MenuTranslations
            .Where(x => x.MenuId == request.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);

            foreach (var translation in translations)
            {
                translation.IsDeleted = true;
            }


            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }

}
