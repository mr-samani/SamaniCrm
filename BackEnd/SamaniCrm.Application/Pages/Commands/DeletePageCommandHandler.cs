using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Pages.Commands
{
    public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand,Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeletePageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeletePageCommand request, CancellationToken cancellationToken)
        {
            var page = await _context.Pages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == request.PageId, cancellationToken);

            if (page is null)
                throw new NotFoundException("Page not found");

            page.IsDeleted = true;
            page.DeletedBy = request.DeletedBy;
            page.DeletedTime = DateTime.UtcNow;

            foreach (var t in page.Translations)
            {
                t.IsDeleted = true;
                t.DeletedBy = request.DeletedBy;
                t.DeletedTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
