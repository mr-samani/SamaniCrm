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
    private readonly IPageService _pageService;

    public CreateOrEditPageMetaDataCommandHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<Guid> Handle(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken)
    {
        return await _pageService.CreatePage(request, cancellationToken);
    }

}

