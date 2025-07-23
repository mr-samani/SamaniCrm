using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Commands;

public record DeleteCustomBlockCommand(Guid Id) : IRequest<Unit>;


public class DeleteCustomBlockCommandHandler : IRequestHandler<DeleteCustomBlockCommand, Unit>
{
    private readonly IPageService _pageService;

    public DeleteCustomBlockCommandHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<Unit> Handle(DeleteCustomBlockCommand request, CancellationToken cancellationToken)
    {
        return await _pageService.DeleteCustomBlockPage(request, cancellationToken);
    }

}