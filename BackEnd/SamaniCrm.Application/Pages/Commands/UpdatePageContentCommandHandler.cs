using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Commands
{
    public class UpdatePageContentCommandHandler : IRequestHandler<UpdatePageContentCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdatePageContentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdatePageContentCommand request, CancellationToken cancellationToken)
        {
            var translation = await _context.PageTranslations
                .FirstOrDefaultAsync(pt => pt.PageId == request.PageId && pt.Culture == request.Culture, cancellationToken);

            if (translation is null)
            {
                translation = new PageTranslation
                {
                    Culture = request.Culture,
                    Data = request.Data,
                    Html = request.Html,
                    Styles = request.Styles,
                    Scripts = request.Scripts,
                };
            }
            else
            {
                translation.Data = request.Data;
                translation.Html = request.Html;
                translation.Styles = request.Styles;
                translation.Scripts = request.Scripts;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
