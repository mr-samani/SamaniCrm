using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs.PageBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Commands;

public class CreateCustomBlockCommand : CustomBlockDto, IRequest<Guid>
{
   
}


public class CreateCustomBlockCommandHandler : IRequestHandler<CreateCustomBlockCommand, Guid>
{
    private readonly IPageService _pageService;

    public CreateCustomBlockCommandHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public Task<Guid> Handle(CreateCustomBlockCommand request, CancellationToken cancellationToken)
    {
        return _pageService.CreateCustomBlock(request, cancellationToken);
    }
}