using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Pages.Commands;
public class CreateOrEditPageMetaDataCommand : PageForEditDto, IRequest<Guid>;

public class CreateOrEditPageMetaDataCommandHandler : IRequestHandler<CreateOrEditPageMetaDataCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOrEditPageMetaDataCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken)
    {

        Page? page;
        if (request.Id.HasValue)
        {
            page = await _context.Pages.FindAsync(request.Id.Value, cancellationToken);
            if (page == null)
            {
                throw new NotFoundException("Page not found");
            }
        }
        else
        {
            page = new Page()
            {
                AuthorId = Guid.Parse(_currentUserService.UserId!),
                CoverImage = request.CoverImage,
                IsActive = request.IsActive,
                IsSystem = false,
                Status = request.Status ?? PageStatusEnum.Draft,
                Type = request.Type,
            };
            foreach (var item in request.Translations ?? [])
            {
                page.Translations.Add(new PageTranslation()
                {
                    Culture = item.Culture,
                    Abstract = item.Abstract,
                    Title = item.Title,
                    Description = item.Description,
                    MetaDescription = item.MetaDescription,
                    MetaKeywords = item.MetaKeywords,
                });
            }
        }
        _context.Pages.Add(page);
        await _context.SaveChangesAsync(cancellationToken);
        return page.Id;
    }

}

