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
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand,Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdatePageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            var translation = await _context.PageTranslations
                .FirstOrDefaultAsync(pt => pt.PageId == request.PageId && pt.Culture == request.Culture, cancellationToken);

            if (translation is null)
                throw new NotFoundException("Page translation not found");

            translation.Title = request.Title;
            translation.Abstract = request.Abstract;
            translation.Description = request.Description;
            translation.Html = request.Html;
            translation.Styles = request.Styles;
            translation.Scripts = request.Scripts;
            translation.LastModifiedTime = DateTime.UtcNow;
            translation.LastModifiedBy = request.LastModifiedBy;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
