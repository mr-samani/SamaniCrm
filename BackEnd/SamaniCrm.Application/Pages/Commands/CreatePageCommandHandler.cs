using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Commands
{
    public class CreatePageCommandHandler : IRequestHandler<CreatePageCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreatePageCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreatePageCommand request, CancellationToken cancellationToken)
        {
            var page = new Page
            {
                Id = Guid.NewGuid(),
                Slag = request.Slag,
                CoverImage = request.CoverImage,
                Status = request.Status,
                CreationTime = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                Translations =
            {
                new PageTranslation
                {
                    Id = Guid.NewGuid(),
                    Culture = request.Culture,
                    Title = request.Title,
                    Abstract = request.Abstract,
                    Description = request.Description,
                    Html = request.Html,
                    Styles = request.Styles,
                    Scripts = request.Scripts,
                    CreationTime = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                }
            }
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync(cancellationToken);
            return page.Id;
        }
    }
}
